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

    public partial class customers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public customers()
        {
            this.cart = new HashSet<cart>();
            this.orders = new HashSet<orders>();
        }

        [Key]
        public int customerId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your full name")]
        public string customerName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your email")]
        public string customerEmail { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone")]
        public string customerPhone { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your password")]
        public string customerPassword { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your address")]
        public string customerAddress { get; set; }
        public string role { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cart> cart { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orders> orders { get; set; }
    }
}
