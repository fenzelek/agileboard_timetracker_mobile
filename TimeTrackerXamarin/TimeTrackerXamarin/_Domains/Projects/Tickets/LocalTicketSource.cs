using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;

namespace TimeTrackerXamarin._Domains.Projects.Tickets
{
    public class LocalTicketSource : ILocalTicketSource
    {

        private readonly SQLiteAsyncConnection db;
        private readonly ILogger logger;

        public LocalTicketSource(IDatabaseConnector connector, ILogger logger)
        {
            this.logger = logger;
            db = connector.Create();
            db.CreateTableAsync<Ticket>();
            db.CreateTableAsync<TicketDetails>();
        }

        public Task<List<Ticket>> GetAll(int projectId)
        {
            return db.Table<Ticket>().Where((ticket) => ticket.project_id == projectId).ToListAsync();
        }

        public async Task<TicketDetails> GetDetails(int ticketId)
        {
            try
            {
                return await db.Table<TicketDetails>().Where((ticket) => ticket.id == ticketId).FirstOrDefaultAsync();
            }
            catch (Exception err)
            {
                logger.Error("There was an error.", err);
                return null;
            }
        }

        public async Task<Ticket> GetTicket(int ticketId)
        {
            try
            {
                var x = await db.Table<Ticket>().Where((ticket) => ticket.id == ticketId).FirstOrDefaultAsync();
                return x;
            }
            catch (Exception err)
            {
                logger.Error("There was an error.", err);
                return null;
            }
        }

        public async Task<bool> SaveTickets(List<Ticket> tickets)
        {
            try
            {
                await db.ExecuteAsync("DELETE FROM Ticket WHERE project_id=?", tickets[0].project_id);
                int rowsAdded;                
                rowsAdded = 0;
                foreach (Ticket ticket in tickets)
                {
                    if(await db.InsertOrReplaceAsync(ticket) == 1)
                        rowsAdded++;
                }
                if (rowsAdded == tickets.Count)
                {
                    return true;
                }
                else
                {                    
                    return false;
                }
            }
            catch(Exception err)
            {
                logger.Error("There was an error.", err);
                return false;
            }
        }

        public async Task<bool> SaveTicketDetails(TicketDetails ticketDetails)
        {
            return await db.InsertOrReplaceAsync(ticketDetails) == 1;
        }
    }
}