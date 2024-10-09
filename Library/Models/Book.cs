using System;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }
        
    public Guid? ReaderId { get; set; }
    public virtual Reader Reader { get; set; }

    public override string ToString()
    {
        return Title;
    }
}

