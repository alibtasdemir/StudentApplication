//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudentApplicationSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class InterviewQuestionPaper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InterviewQuestionPaper()
        {
            this.Applications = new HashSet<Application>();
        }
        
        [Key]
        [DisplayName("Interview Paper")]
        public int paperId { get; set; }
        public Nullable<int> applicationId { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<int> jobId { get; set; }
        [DisplayName("Question 1")]
        public Nullable<int> question1 { get; set; }
        [DisplayName("Question 2")]
        public Nullable<int> question2 { get; set; }
        [DisplayName("Question 3")]
        public Nullable<int> question3 { get; set; }
        [DisplayName("Answer 1")]
        public string answer1 { get; set; }
        [DisplayName("Answer 2")]
        public string answer2 { get; set; }
        [DisplayName("Answer 3")]
        public string answer3 { get; set; }
        [DisplayName("Created by")]
        public Nullable<int> cd_creater { get; set; }
        [DisplayName("Created")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> dt_created { get; set; }
        [DisplayName("Modified by")]
        public Nullable<int> cd_modifier { get; set; }
        [DisplayName("Modified")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> dt_modified { get; set; }
    
        public virtual Application Application { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Application> Applications { get; set; }
        public virtual User User { get; set; }
        public virtual Question Question { get; set; }
        public virtual Question Question4 { get; set; }
        public virtual Question Question5 { get; set; }
    }
}