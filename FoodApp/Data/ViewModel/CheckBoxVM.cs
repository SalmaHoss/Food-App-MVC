using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodApp.Data.ViewModel
{
    [NotMapped]
    public class CheckBoxVM
    {
       // List<City> cites { get; set; }
        string[] categories { get; set; }
    }
}