using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;

namespace RMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;

        public SalesViewModel(IProductEndpoint productEndpoint)
        {
            _productEndpoint = productEndpoint;
            
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            Products = new BindingList<UIProductModel>(productList);
        }

        private BindingList<UIProductModel> _products;

        public BindingList<UIProductModel> Products
        {
            get { return _products; }
            set
            {
                _products = value; 
                NotifyOfPropertyChange(() => Products);
            }
        }

        private BindingList<string> _cart;

        public BindingList<string> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private int _itemQuantity;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

        public string SubTotal
        {
            get
            {
                // TODO - Replace with calculation
                return "$0.00";
            }
          
        }

        public string Tax
        {
            get
            {
                // TODO - Replace with calculation
                return "$0.00";
            }

        }

        public string Total
        {
            get
            {
                // TODO - Replace with calculation
                return "$0.00";
            }

        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                //Make sure something is selected and ife there is an item quantity
                

                return output;
            }
        }

        public bool RemoveFromCart
        {
            get
            {
                bool output = false;

                //Make sure something is selected 


                return output;
            }
        }

        public void AddToCart()
        {
            
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //Make sure something is something in the cart


                return output;
            }
        }

        public void CheckOut()
        {

        }

    }
}
