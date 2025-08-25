using AutoMapper;
using DataAccess.Services.Implements;
using DataAccess.Services.Interfaces;
using DataAccess.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Ultitity.Email.Interface;

namespace DataAccess.Services.FacadeService
{
    public class FacadeService : IFacadeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailQueue _emailQueue;
        private readonly IMapper _mapper;

        public ICategoryService CategoryService { get; }
        public IProductService ProductService { get; }
        public IShoppingCartService ShoppingCartService { get; }
        public ICartItemService CartItemService { get; }

        public FacadeService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IEmailQueue emailQueue,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailQueue = emailQueue;
            _mapper = mapper;
            CategoryService = new CategoryService(_unitOfWork, _mapper);
            ProductService = new ProductService(_unitOfWork, _mapper);
            ShoppingCartService = new ShoppingCartService(_unitOfWork, _mapper);
            CartItemService = new CartItemService(_unitOfWork, _mapper);
        }
    }
}
