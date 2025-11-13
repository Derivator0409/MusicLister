using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLister.Models
{
    public class Song
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(150)]
        [Required]
        public string Title { get; set; }=string.Empty;
        [StringLength(150)]
        [Required]
        public TimeSpan Length { get; set; }
        [Required]
        public int ArtistID { get; set; }

    
    
    
    }
}
