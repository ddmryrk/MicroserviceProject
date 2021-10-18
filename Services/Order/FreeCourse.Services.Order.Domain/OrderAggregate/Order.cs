using System;
using System.Collections.Generic;
using System.Linq;
using FreeCourse.Services.Order.Domain.Core;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    ///Ef core features
    ///--Owned Types
    ///--Shadow Property
    ///--Backing Field
    public class Order : Entity, IAggregateRoot
    {
        public DateTime CreateDate { get; private set; }
        ///Owned olarak tanımlanırsa ef core tarafında address için ayrı bir tablo oluşturur ve order ile ilişkilendirir.
        ///Owned olarak tanımlanmazsa order tablosu içinde address propertylerini kolon olarak ekler
        public Address Address { get; private set; }
        public string BuyerId { get; private set; }
        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        public Order()
        {

        }

        public Order(string buyerId, Address address)
        {
            _orderItems = new List<OrderItem>();
            CreateDate = DateTime.Now;
            BuyerId = buyerId;
            Address = address;
        }

        public void AddOrderItem(string productId, string productName, decimal price, string pictureUrl)
        {
            var existProduct = _orderItems.Any(x => x.ProductId == productId);
            if (!existProduct)
            {
                var newOrderItem = new OrderItem(productId, productName, pictureUrl, price);
                _orderItems.Add(newOrderItem);
            }
        }

        public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);
    }
}
