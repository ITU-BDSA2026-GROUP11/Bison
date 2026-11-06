using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bison.Core.Models;

public class Author : IdentityUser<int>
{

    [NotMapped]
    public int AuthorId
    {
        get => Id;
        set => Id = value;
    }

    public string Name { get; set; } = "";

    // Email is now inherited from IdentityUser<int>.

    public List<Post> Posts { get; set; } = new();
}
