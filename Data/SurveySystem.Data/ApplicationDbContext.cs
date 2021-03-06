﻿namespace SurveySystem.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    using Common.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    using SurveySystem.Data.Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", false)
        {
        }

        public IDbSet<Survey> Surveys { get; set; }

        public IDbSet<Question> Questions { get; set; }

        public IDbSet<RespondentAnswer> RespondentAnswers { get; set; }

        public IDbSet<QuestionAnswer> QuestionAnswers { get; set; }

        public IDbSet<Respondent> Respondents { get; set; }

        public IDbSet<Submission> Submissions { get; set; }

        public IDbSet<SubmissionCode> SubmissionCodes { get; set; }

        public override int SaveChanges()
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        private void ApplyAuditInfoRules()
        {
            // Approach via @julielerman: http://bit.ly/123661P
            foreach (var entry in
                this.ChangeTracker.Entries()
                    .Where(
                        e =>
                        e.Entity is IAuditInfo && ((e.State == EntityState.Added) || (e.State == EntityState.Modified))))
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default(DateTime))
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}
