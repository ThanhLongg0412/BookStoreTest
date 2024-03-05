using bookstore.Models;

namespace BookStore.Models
{
    public partial class Order
    {
        public int Id { get; set; }

        public string Status { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public int? PaymentMethodId { get; set; }

        public int? AdminId { get; set; }

        public int? CustomerId { get; set; }

        public virtual Admin? Admin { get; set; }

        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual PaymentMethod? PaymentMethod { get; set; }
    }
}
