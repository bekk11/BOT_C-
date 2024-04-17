using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOT.Entities;

public class MyUserConfigurations : IEntityTypeConfiguration<MyUser>
{
    public void Configure(EntityTypeBuilder<MyUser> builder)
    {
        builder.HasKey(b => b.UserId);
        builder.HasIndex(b => b.ChatId).IsUnique(false);
        builder.Property(b => b.Firstname).HasMaxLength(255);
        builder.Property(b => b.Lastname).HasMaxLength(255);
        builder.Property(b => b.LanguageCode).HasMaxLength(3).IsRequired();
    }
}