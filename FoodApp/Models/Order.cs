using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodApp.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public List<OrderItem> OrderItems { get; set; }

    }
}
