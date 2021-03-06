//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WatchStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public orders()
        {
            this.order_details = new HashSet<order_details>();
        }
    
        public int orderId { get; set; }
        public int fk_customerId { get; set; }
        public double totalPrice { get; set; }
        public string orderDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your full name")]
        public string fullName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
        public string phoneNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the complete address detail")]
        public string addressDetail { get; set; }
        public int orderStatus { get; set; }
    
        public virtual customers customers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<order_details> order_details { get; set; }
    }
}
