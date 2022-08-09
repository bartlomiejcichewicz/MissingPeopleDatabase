using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissingPeopleDatabase.Models
{
    public class Person
    {
        [Key]
        [StringLength(6)]
        public string Id { get; set; }

        [Required]
        [StringLength(75)]
        public String FirstName { get; set; }

        [Required]
        [StringLength(255)]
        public String LastName { get; set; }

        [Required]
        [ForeignKey("Sexes")]
        [Display(Name = "Sex")]
        public int SexId { get; set; }
        public virtual Sex Sexes { get; set; }

        [ForeignKey("Cities")]
        [Display(Name = "City")]
        public int? CityId { get; set; }
        public virtual City Cities { get; set; }

        [ForeignKey("Statuses")]
        [Display(Name = "Status")]
        public int? StatusId { get; set; }
        public virtual Status Statuses { get; set; }
        public string PhotoUrl { get; set; } = "noimage.png";

        [Display(Name = "Person Photo")]
        [NotMapped]
        public IFormFile PersonPhoto { get; set; }

        [NotMapped]
        public string BreifPhotoName { get; set; }

    }
}
