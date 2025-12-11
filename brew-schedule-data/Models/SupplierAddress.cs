using System;
using System.Collections.Generic;

namespace brew_schedule_data.Models;

public partial class SupplierAddress
{
    public int SupplierId { get; set; }

    public int AddressId { get; set; }

    public int AddressTypeId { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
}
