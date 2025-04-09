using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API.Inventory.Model;
using API.Inventory.Repository;
using Dapper;
using Npgsql;

namespace API.Inventory.Data.RepositoryService
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IDbConnection _dbConnection;

        public BookingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Members>> GetMembersAsync()
        {
             EnsureConnectionOpenAsync();
            var query = "SELECT * FROM members";
            return await _dbConnection.QueryAsync<Members>(query);
        }

        public async Task<Members> GetMemberByIdAsync(int id)
        {
             EnsureConnectionOpenAsync();
            var query = $"SELECT * FROM members WHERE id = {id}";
            return await _dbConnection.QueryFirstOrDefaultAsync<Members>(query);
        }

        public async Task<List<InventoryDto>> GetAvailableInventoryAsync()
        {
             EnsureConnectionOpenAsync();
            var query = @"
            SELECT id, title, description, remaining_count AS RemainingCount, expiration_date AS ExpirationDate
            FROM inventory
            WHERE remaining_count > 0 AND expiration_date > @CurrentDate";

            var inventoryList = await _dbConnection.QueryAsync<InventoryDto>(query, new { CurrentDate = DateTime.Now });
            return inventoryList.ToList();
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryAsync()
        {
            EnsureConnectionOpenAsync();
            var query = "SELECT * FROM inventory";
            return await _dbConnection.QueryAsync<InventoryDto>(query);
        }

        public async Task<bool> BookItemAsync(List<BookingCsvRecord> bookingCsvRecords)
        {
             EnsureConnectionOpenAsync();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    foreach (var record in bookingCsvRecords)
                    {
                        try
                        {
                            var memberQuery = "SELECT bookingcount FROM members WHERE id = @MemberId";
                            var inventoryQuery = "SELECT remainingcount FROM inventory WHERE id = @InventoryId";

                            var member = await _dbConnection.QueryFirstOrDefaultAsync<Members>(memberQuery, new { MemberId = record.MemberId }, transaction);
                            var inventory = await _dbConnection.QueryFirstOrDefaultAsync<InventoryDto>(inventoryQuery, new { InventoryId = record.InventoryId }, transaction);

                            if (member.BookingCount >= 2 || inventory.RemainingCount <= 0)
                            {
                                continue;
                            }

                            var bookingReference = Guid.NewGuid().ToString();

                            var bookQuery = @"
                        UPDATE members SET bookingcount = bookingcount + 1 WHERE id = @MemberId;
                        UPDATE inventory SET remainingcount = remainingcount - 1 WHERE id = @InventoryId;
                        ";

                            var affectedRows = await _dbConnection.ExecuteAsync(bookQuery, new { MemberId = record.MemberId, InventoryId = record.InventoryId }, transaction);

                            if (affectedRows <= 0)
                            {
                                continue;
                            }

                            var insertBookingQuery = @"
                        INSERT INTO bookings (memberid, inventoryid, bookingreference, bookingdate, status)
                        VALUES (@MemberId, @InventoryId, @BookingReference, @BookingDate, @BookingStatus);
                        ";

                            var bookingInserted = await _dbConnection.ExecuteAsync(insertBookingQuery, new
                            {
                                MemberId = record.MemberId,
                                InventoryId = record.InventoryId,
                                BookingReference = bookingReference,
                                BookingDate = DateTime.UtcNow,
                                BookingStatus = record.BookingStatus
                            }, transaction);

                            if (bookingInserted <= 0)
                            {
                                continue;
                            }
                        }
                        catch(Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> CancelBookingAsync(int memberId, int inventoryId)
        {
            EnsureConnectionOpenAsync();
            var cancelQuery = @"
            UPDATE members SET booking_count = booking_count - 1 WHERE id = @MemberId;
            UPDATE inventory SET remaining_count = remaining_count + 1 WHERE id = @InventoryId;
            ";

            var affectedRows = await _dbConnection.ExecuteAsync(cancelQuery, new { MemberId = memberId, InventoryId = inventoryId });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            var query = @"
    SELECT 
        b.memberid, 
        m.name AS membername,  -- Assuming 'name' is the member's name in the 'members' table
        b.inventoryid, 
        i.title AS inventoryname,  -- Assuming 'title' is the inventory's title in the 'inventory' table
        b.bookingreference, 
        b.bookingdate, 
        b.status
    FROM bookings b
    JOIN members m ON b.memberid = m.id
    JOIN inventory i ON b.inventoryid = i.id;";

            // Execute the query and map the result to the Booking model
            var bookings = await _dbConnection.QueryAsync<Booking>(query);

            return bookings;
        }


        private void EnsureConnectionOpenAsync()
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                 _dbConnection.Open();
            }
        }
    }
}
