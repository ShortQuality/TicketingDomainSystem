
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;
using TicketingSystem.DAL.Repositories;

namespace TicketingSystem.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private TicketingSystemContext _dbContext;
        private IRepository<Cart> _CartsRepository;
        private IRepository<CartDetail> _CartDetailsRepository;
        private IRepository<Event> _EventsRepository;
        private IRepository<Payment> _PaymentsRepository;
        private IRepository<Price> _PricesRepository;
        private IRepository<Seat> _SeatsRepository;
        private IRepository<SeatRow> _SeatRowsRepository;
        private IRepository<Section> _SectionsRepository;
        private IRepository<Venue> _VenuesRepository;
        private bool _disposed;

        public UnitOfWork(TicketingSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<Cart> CartsRepository => _CartsRepository ??= new Repository<Cart>(_dbContext.Carts);
        public IRepository<CartDetail> CartDetailsRepository => _CartDetailsRepository ??= new Repository<CartDetail>(_dbContext.CartDetails);
        public IRepository<Event> EventsRepository => _EventsRepository ??= new Repository<Event>(_dbContext.Events);
        public IRepository<Payment> PaymentsRepository => _PaymentsRepository ??= new Repository<Payment>(_dbContext.Payments);
        public IRepository<Price> PricesRepository => _PricesRepository ??= new Repository<Price>(_dbContext.Prices);
        public IRepository<Seat> SeatsRepository => _SeatsRepository ??= new Repository<Seat>(_dbContext.Seats);
        public IRepository<SeatRow> SeatRowsRepository => _SeatRowsRepository ??= new Repository<SeatRow>(_dbContext.SeatRows);
        public IRepository<Section> SectionsRepository => _SectionsRepository ??= new Repository<Section>(_dbContext.Sections);
        public IRepository<Venue> VenuesRepository => _VenuesRepository ??= new Repository<Venue>(_dbContext.Venues);


        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
