using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using RMDesktopUI.Models;

namespace RMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;
        private IConfigHelper _configHelper;
        private ISaleEndpoint _saleEndpoint;
        private IMapper _mapper;
        private readonly StatusInfoViewModel _statusInfo;
        private readonly IWindowManager _windowManager;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, 
            ISaleEndpoint saleEndpoint, IMapper mapper, StatusInfoViewModel statusInfo, IWindowManager windowManager)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            _statusInfo = statusInfo;
            _windowManager = windowManager;
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unauthorized")
                {
                    _statusInfo.UpdateMessage("Unauthorized Access", "You do not have permission to interact with this part of application.");
                    await _windowManager.ShowDialogAsync(_statusInfo, null, settings);
                }
                else
                {
                    _statusInfo.UpdateMessage("Fatal Exception", ex.Message);
                    await _windowManager.ShowDialogAsync(_statusInfo, null, settings);
                }

                await TryCloseAsync();
            }
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            var products = _mapper.Map<List<UIProductDisplayModel>>(productList);
            Products = new BindingList<UIProductDisplayModel>(products);
        }


        private UIProductDisplayModel _selectedProductDisplay;

        public UIProductDisplayModel SelectedProductDisplay
        {
            get { return _selectedProductDisplay; }
            set
            {
                _selectedProductDisplay = value;
                NotifyOfPropertyChange(() => SelectedProductDisplay);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private BindingList<UIProductDisplayModel> _products;

        public BindingList<UIProductDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();

            // TODO - Add clearing the selectedCartItem if it does not doing itself

            await LoadProducts();

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        private CartItemDisplayModel _selectedCartItem;

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }


        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                subTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }

            return subTotal;
        }
        public string SubTotal
        {
            get
            {
                return CalculateSubTotal().ToString("C");
            }

        }

        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate()/100;

            taxAmount = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

            //foreach (var item in Cart)
            //{
            //    if (item.ProductDisplay.IsTaxable)
            //    {
            //        taxAmount += (item.ProductDisplay.RetailPrice * item.QuantityInCart * taxRate);
            //    }
            //}

            return taxAmount;
        }

        public string Tax
        {
            get
            {
                return CalculateTax().ToString("C");
            }

        }


        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                return total.ToString("C");
            }

        }

        public bool CanAddToCart
        {
            
            get
            {
                bool output = false;
                
                if (ItemQuantity > 0 && SelectedProductDisplay?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }
                

                return output;
            }
        }
        public void AddToCart()
        {
            CartItemDisplayModel existingItemDisplay = Cart.FirstOrDefault(x => x.Product == SelectedProductDisplay);

            if (existingItemDisplay != null)
            {
                existingItemDisplay.QuantityInCart += ItemQuantity;

            }
            else
            {
                CartItemDisplayModel itemDisplay = new CartItemDisplayModel
                {
                    Product = SelectedProductDisplay,
                    QuantityInCart = ItemQuantity
                };

                Cart.Add(itemDisplay);
            }

            SelectedProductDisplay.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            //NotifyOfPropertyChange(() => existingItemDisplay.DisplayText);

        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                if (SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QuantityInStock += 1;

            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart -= 1;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }
     

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //Make sure something is something in the cart
                if (Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task CheckOut()
        {
            SaleModel sale = new SaleModel();

            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndpoint.PostSale(sale);

            await ResetSalesViewModel();
        }

    }
}
