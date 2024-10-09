using System;
using System.Collections.Generic;

public class Reader
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
        
    public string FullName => $"{FirstName} {LastName}";
    public override string ToString()
    {
        return FullName;
    }
    public virtual ICollection<Book> Books { get; set; } = new List<Book>(); 
}

