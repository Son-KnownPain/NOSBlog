//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NOSBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class blog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public blog()
        {
            this.comments = new HashSet<comment>();
            this.user_like_blogs = new HashSet<user_like_blogs>();
        }
    
        public int id { get; set; }
        public Nullable<int> user_id { get; set; }
        [Required(ErrorMessage = "Title is require")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string title { get; set; }
        [DisplayName("Summary")]
        [Required(ErrorMessage = "Summary is require")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string summary { get; set; }
        [DisplayName("Content")]
        [Required(ErrorMessage = "Content is require")]
        [MaxLength(4000, ErrorMessage = "Max length is 4000 characters")]
        public string content { get; set; }
        [DisplayName("Thumbnail")]
        public string thumbnail { get; set; }
        public int like_count { get; set; }
        public int comment_count { get; set; }
        public bool @lock { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    
        public virtual user user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comment> comments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<user_like_blogs> user_like_blogs { get; set; }
    }
}
