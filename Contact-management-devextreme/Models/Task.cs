using System;
using System.Collections.Generic;

namespace Contact_management_devextreme.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public string Priority { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int AssignedToId { get; set; }

    public virtual Contact AssignedTo { get; set; } = null!;
}
