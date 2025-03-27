using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BackMange.Models;

public partial class GameCaseContext : DbContext
{
    public GameCaseContext(DbContextOptions<GameCaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAdmin> TAdmins { get; set; }

    public virtual DbSet<TAdminStatus> TAdminStatuses { get; set; }

    public virtual DbSet<TAnnounce> TAnnounces { get; set; }

    public virtual DbSet<TAnnounceCategory> TAnnounceCategories { get; set; }

    public virtual DbSet<TCategory> TCategories { get; set; }

    public virtual DbSet<TChat> TChats { get; set; }

    public virtual DbSet<TConfirmReply> TConfirmReplys { get; set; }

    public virtual DbSet<TEcpayOrder> TEcpayOrders { get; set; }

    public virtual DbSet<TImage> TImages { get; set; }

    public virtual DbSet<TMessagesHistory> TMessagesHistories { get; set; }

    public virtual DbSet<TPortfolio> TPortfolios { get; set; }

    public virtual DbSet<TPoster> TPosters { get; set; }

    public virtual DbSet<TTask> TTasks { get; set; }

    public virtual DbSet<TTaskFollow> TTaskFollows { get; set; }

    public virtual DbSet<TUser> TUsers { get; set; }

    public virtual DbSet<TUserNotification> TUserNotifications { get; set; }

    public virtual DbSet<TUserType> TUserTypes { get; set; }

    public virtual DbSet<TVerification> TVerifications { get; set; }

    public virtual DbSet<TWorker> TWorkers { get; set; }

    public virtual DbSet<TWorkerFollow> TWorkerFollows { get; set; }

    public virtual DbSet<Ttransaction> Ttransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TAdmin>(entity =>
        {
            entity.HasKey(e => e.FadminId).HasName("PK__tAdmins__F5DAD0F483A608BD");

            entity.ToTable("tAdmins");

            entity.HasIndex(e => e.Femail, "UQ__tAdmins__0F13454E21FF7EC9").IsUnique();

            entity.HasIndex(e => e.FadminNo, "UQ__tAdmins__F5DB394E615B365D").IsUnique();

            entity.Property(e => e.FadminId).HasColumnName("FAdminID");
            entity.Property(e => e.FadmPassword)
                .HasMaxLength(255)
                .HasColumnName("FAdmPassword");
            entity.Property(e => e.FadminLevel).HasColumnName("FAdminLevel");
            entity.Property(e => e.FadminNo)
                .HasMaxLength(20)
                .HasColumnName("FAdminNo");
            entity.Property(e => e.FcreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FCreatedAt");
            entity.Property(e => e.Femail)
                .HasMaxLength(100)
                .HasColumnName("FEmail");
            entity.Property(e => e.FfullName)
                .HasMaxLength(100)
                .HasColumnName("FFullName");
            entity.Property(e => e.FmobilePhone)
                .HasMaxLength(20)
                .HasColumnName("FMobilePhone");
            entity.Property(e => e.FstatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("FStatusID");
            entity.Property(e => e.FupdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("FUpdatedAt");

            entity.HasOne(d => d.Fstatus).WithMany(p => p.TAdmins)
                .HasForeignKey(d => d.FstatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_Status");
        });

        modelBuilder.Entity<TAdminStatus>(entity =>
        {
            entity.HasKey(e => e.FstatusId).HasName("PK__tAdminSt__08155345D9CA2DB7");

            entity.ToTable("tAdminStatus");

            entity.Property(e => e.FstatusId).HasColumnName("FStatusID");
            entity.Property(e => e.FcreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FCreatedAt");
            entity.Property(e => e.Fdescription)
                .HasMaxLength(100)
                .HasColumnName("FDescription");
            entity.Property(e => e.FstatusName)
                .HasMaxLength(20)
                .HasColumnName("FStatusName");
        });

        modelBuilder.Entity<TAnnounce>(entity =>
        {
            entity.HasKey(e => e.FAnnounceId).HasName("PK__tAnnounc__1D5ECEB9949963AD");

            entity.ToTable("tAnnounce");

            entity.Property(e => e.FAnnounceId).HasColumnName("fAnnounceId");
            entity.Property(e => e.FCategoryId).HasColumnName("fCategoryId");
            entity.Property(e => e.FContent)
                .HasColumnType("text")
                .HasColumnName("fContent");
            entity.Property(e => e.FCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fCreatedAt");
            entity.Property(e => e.FPriority)
                .HasDefaultValue(1)
                .HasColumnName("fPriority");
            entity.Property(e => e.FTitle)
                .HasMaxLength(100)
                .HasColumnName("fTitle");
            entity.Property(e => e.FUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fUpdatedAt");
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasOne(d => d.FCategory).WithMany(p => p.TAnnounces)
                .HasForeignKey(d => d.FCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tAnnounce__fCate__02FC7413");
        });

        modelBuilder.Entity<TAnnounceCategory>(entity =>
        {
            entity.HasKey(e => e.FCategoryId).HasName("PK__tAnnounc__53E607B374B18B2A");

            entity.ToTable("tAnnounceCategories");

            entity.HasIndex(e => e.FCategoryName, "UQ__tAnnounc__11826C736068E7A4").IsUnique();

            entity.Property(e => e.FCategoryId).HasColumnName("fCategoryId");
            entity.Property(e => e.FCategoryName)
                .HasMaxLength(50)
                .HasColumnName("fCategoryName");
        });

        modelBuilder.Entity<TCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tCategor__3214EC27CE92F09C");

            entity.ToTable("tCategory");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.JobName).HasMaxLength(255);
        });

        modelBuilder.Entity<TChat>(entity =>
        {
            entity.HasKey(e => e.FChatId).HasName("PK__tChats__81C1C47F55048BBD");

            entity.ToTable("tChats");

            entity.Property(e => e.FChatId).HasColumnName("fChatId");
            entity.Property(e => e.FCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fCreatedAt");
            entity.Property(e => e.FPosterId).HasColumnName("fPosterId");
            entity.Property(e => e.FWorkerId).HasColumnName("fWorkerId");

            entity.HasOne(d => d.FPoster).WithMany(p => p.TChatFPosters)
                .HasForeignKey(d => d.FPosterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tChats__fPosterI__0B91BA14");

            entity.HasOne(d => d.FWorker).WithMany(p => p.TChatFWorkers)
                .HasForeignKey(d => d.FWorkerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tChats__fWorkerI__0A9D95DB");
        });

        modelBuilder.Entity<TConfirmReply>(entity =>
        {
            entity.HasKey(e => e.FconfirmReplyId).HasName("PK__tConfirm__41F322F664A106CE");

            entity.ToTable("tConfirmReplys");

            entity.Property(e => e.FconfirmReplyId).HasColumnName("FConfirmReplyID");
            entity.Property(e => e.FconfirmationStatus)
                .HasMaxLength(20)
                .HasDefaultValue("待確認")
                .HasColumnName("FConfirmation_status");
            entity.Property(e => e.FconfirmationType)
                .HasMaxLength(20)
                .HasColumnName("FConfirmation_type");
            entity.Property(e => e.FposterId).HasColumnName("FPosterID");
            entity.Property(e => e.Fremarks)
                .HasColumnType("text")
                .HasColumnName("FRemarks");
            entity.Property(e => e.FtaskId).HasColumnName("FTaskID");
            entity.Property(e => e.FworkerId).HasColumnName("FWorkerID");

            entity.HasOne(d => d.Fposter).WithMany(p => p.TConfirmReplies)
                .HasForeignKey(d => d.FposterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tConfirmR__FPost__208CD6FA");

            entity.HasOne(d => d.Ftask).WithMany(p => p.TConfirmReplies)
                .HasForeignKey(d => d.FtaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tConfirmR__FTask__1F98B2C1");

            entity.HasOne(d => d.Fworker).WithMany(p => p.TConfirmReplies)
                .HasForeignKey(d => d.FworkerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tConfirmR__FWork__2180FB33");
        });

        modelBuilder.Entity<TEcpayOrder>(entity =>
        {
            entity.HasKey(e => e.MerchantTradeNo).HasName("PK_EcpayOrders");

            entity.ToTable("tEcpayOrders");

            entity.Property(e => e.MerchantTradeNo).HasMaxLength(50);
            entity.Property(e => e.FposterId).HasColumnName("FPosterID");
            entity.Property(e => e.FtaskId).HasColumnName("FTaskID");
            entity.Property(e => e.FworkerId).HasColumnName("FWorkerID");
            entity.Property(e => e.MemberId)
                .HasMaxLength(50)
                .HasColumnName("MemberID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentType).HasMaxLength(50);
            entity.Property(e => e.PaymentTypeChargeFee).HasMaxLength(50);
            entity.Property(e => e.RtnMsg).HasMaxLength(50);
            entity.Property(e => e.TradeDate).HasMaxLength(50);
            entity.Property(e => e.TradeNo).HasMaxLength(50);

            entity.HasOne(d => d.Fposter).WithMany(p => p.TEcpayOrderFposters)
                .HasForeignKey(d => d.FposterId)
                .HasConstraintName("FK_EcpayOrders_Poster");

            entity.HasOne(d => d.Ftask).WithMany(p => p.TEcpayOrders)
                .HasForeignKey(d => d.FtaskId)
                .HasConstraintName("FK_EcpayOrders_Tasks");

            entity.HasOne(d => d.Fworker).WithMany(p => p.TEcpayOrderFworkers)
                .HasForeignKey(d => d.FworkerId)
                .HasConstraintName("FK_EcpayOrders_Worker");
        });

        modelBuilder.Entity<TImage>(entity =>
        {
            entity.HasKey(e => e.FimageId).HasName("PK__tImages__AD9ECEA727C722AD");

            entity.ToTable("tImages");

            entity.Property(e => e.FimageId).HasColumnName("FImageID");
            entity.Property(e => e.Fcategory)
                .HasMaxLength(50)
                .HasColumnName("FCategory");
            entity.Property(e => e.FcreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FCreatedAt");
            entity.Property(e => e.FimageName)
                .HasMaxLength(255)
                .HasColumnName("FImageName");
            entity.Property(e => e.FimagePath)
                .HasMaxLength(255)
                .HasColumnName("FImagePath");
            entity.Property(e => e.FisMain)
                .HasDefaultValue(false)
                .HasColumnName("FIsMain");
            entity.Property(e => e.Frole)
                .HasMaxLength(20)
                .HasColumnName("FRole");
            entity.Property(e => e.FuserId).HasColumnName("FUserID");

            entity.HasOne(d => d.Fuser).WithMany(p => p.TImages)
                .HasForeignKey(d => d.FuserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tImages_tUsers");
        });

        modelBuilder.Entity<TMessagesHistory>(entity =>
        {
            entity.HasKey(e => e.FMessageId).HasName("PK__tMessage__D660247EDA7294ED");

            entity.ToTable("tMessagesHistory");

            entity.Property(e => e.FMessageId).HasColumnName("fMessageId");
            entity.Property(e => e.FChatId).HasColumnName("fChatId");
            entity.Property(e => e.FIsRead).HasColumnName("fIsRead");
            entity.Property(e => e.FMessageText).HasColumnName("fMessageText");
            entity.Property(e => e.FSenderId).HasColumnName("fSenderId");
            entity.Property(e => e.FSentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fSentAt");

            entity.HasOne(d => d.FChat).WithMany(p => p.TMessagesHistories)
                .HasForeignKey(d => d.FChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tMessages__fChat__10566F31");

            entity.HasOne(d => d.FSender).WithMany(p => p.TMessagesHistories)
                .HasForeignKey(d => d.FSenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tMessages__fSend__114A936A");
        });

        modelBuilder.Entity<TPortfolio>(entity =>
        {
            entity.HasKey(e => e.PortfolioId).HasName("PK__tPortfol__6D3A139D08703CAA");

            entity.ToTable("tPortfolios");

            entity.Property(e => e.PortfolioId).HasColumnName("PortfolioID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FuserId).HasColumnName("FUserID");
            entity.Property(e => e.MediaUrl)
                .HasMaxLength(255)
                .HasColumnName("MediaURL");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Fuser).WithMany(p => p.TPortfolios)
                .HasForeignKey(d => d.FuserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Portfolios_Workers");
        });

        modelBuilder.Entity<TPoster>(entity =>
        {
            entity.HasKey(e => e.FuserId).HasName("PK__tPosters__A5C5AA2B07D308E4");

            entity.ToTable("tPosters");

            entity.Property(e => e.FuserId)
                .ValueGeneratedNever()
                .HasColumnName("FUserID");
            entity.Property(e => e.FcompanyName)
                .HasMaxLength(255)
                .HasColumnName("FCompanyName");
            entity.Property(e => e.FcompanyRegistrationNumber)
                .HasMaxLength(50)
                .HasColumnName("FCompanyRegistrationNumber");
            entity.Property(e => e.FcreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FCreatedAt");
            entity.Property(e => e.FisDeleted)
                .HasDefaultValue(false)
                .HasColumnName("FIsDeleted");
            entity.Property(e => e.FisVerified)
                .HasDefaultValue(false)
                .HasColumnName("FIsVerified");
            entity.Property(e => e.FreputationScore).HasColumnName("FReputationScore");
            entity.Property(e => e.FupdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("FUpdatedAt");

            entity.HasOne(d => d.Fuser).WithOne(p => p.TPoster)
                .HasForeignKey<TPoster>(d => d.FuserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posters_Users");
        });

        modelBuilder.Entity<TTask>(entity =>
        {
            entity.HasKey(e => e.FtaskId).HasName("PK__tTasks__807089A78E33F27E");

            entity.ToTable("tTasks");

            entity.Property(e => e.FtaskId).HasColumnName("FTaskID");
            entity.Property(e => e.Fbudget).HasColumnName("FBudget");
            entity.Property(e => e.FcategoryId).HasColumnName("FCategoryID");
            entity.Property(e => e.FcreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FCreatedAt");
            entity.Property(e => e.Fdeadline)
                .HasColumnType("datetime")
                .HasColumnName("FDeadline");
            entity.Property(e => e.Fdescription).HasColumnName("FDescription");
            entity.Property(e => e.Femail)
                .HasMaxLength(50)
                .HasColumnName("FEmail");
            entity.Property(e => e.Flocation)
                .HasMaxLength(255)
                .HasColumnName("FLocation");
            entity.Property(e => e.FlocationDetail)
                .HasMaxLength(255)
                .HasColumnName("FLocationDetail");
            entity.Property(e => e.Fmember)
                .HasMaxLength(50)
                .HasColumnName("FMember");
            entity.Property(e => e.Fphone)
                .HasMaxLength(50)
                .HasColumnName("FPhone");
            entity.Property(e => e.FposterId).HasColumnName("FPosterID");
            entity.Property(e => e.Fstatus)
                .HasMaxLength(20)
                .HasColumnName("FStatus");
            entity.Property(e => e.Ftitle)
                .HasMaxLength(255)
                .HasColumnName("FTitle");
            entity.Property(e => e.FupdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FUpdatedAt");
        });

        modelBuilder.Entity<TTaskFollow>(entity =>
        {
            entity.HasKey(e => e.FfollowId).HasName("PK__tTaskFol__A894E15406D392FB");

            entity.ToTable("tTaskFollows");

            entity.HasIndex(e => new { e.FfollowerId, e.FtaskId }, "UQ_TaskFollows").IsUnique();

            entity.Property(e => e.FfollowId).HasColumnName("FFollowID");
            entity.Property(e => e.FcreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FCreatedAt");
            entity.Property(e => e.FfollowerId).HasColumnName("FFollowerID");
            entity.Property(e => e.FtaskId).HasColumnName("FTaskID");

            entity.HasOne(d => d.Ffollower).WithMany(p => p.TTaskFollows)
                .HasForeignKey(d => d.FfollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskFollows_Follower");

            entity.HasOne(d => d.Ftask).WithMany(p => p.TTaskFollows)
                .HasForeignKey(d => d.FtaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskFollows_Task");
        });

        modelBuilder.Entity<TUser>(entity =>
        {
            entity.HasKey(e => e.FuserId).HasName("PK__tUsers__A5C5AA2BC6AD4058");

            entity.ToTable("tUsers");

            entity.HasIndex(e => e.FgoogleId, "IX_Users_GoogleID")
                .IsUnique()
                .HasFilter("([FGoogleID] IS NOT NULL)");

            entity.HasIndex(e => e.Femail, "UQ__tUsers__0F13454E2908F557").IsUnique();

            entity.Property(e => e.FuserId).HasColumnName("FUserID");
            entity.Property(e => e.Faddress)
                .HasMaxLength(255)
                .HasColumnName("FAddress");
            entity.Property(e => e.Fbirthday).HasColumnName("FBirthday");
            entity.Property(e => e.FcompanyNumber)
                .HasMaxLength(8)
                .HasColumnName("FCompanyNumber");
            entity.Property(e => e.FcreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FCreatedAt");
            entity.Property(e => e.Femail)
                .HasMaxLength(100)
                .HasColumnName("FEmail");
            entity.Property(e => e.FfullName)
                .HasMaxLength(100)
                .HasColumnName("FFullName");
            entity.Property(e => e.Fgender)
                .HasMaxLength(10)
                .HasColumnName("FGender");
            entity.Property(e => e.FgoogleId)
                .HasMaxLength(100)
                .HasColumnName("FGoogleID");
            entity.Property(e => e.FidNumber)
                .HasMaxLength(10)
                .HasColumnName("FIdNumber");
            entity.Property(e => e.FisEmailVerified)
                .HasDefaultValue(false)
                .HasColumnName("FIsEmailVerified");
            entity.Property(e => e.FlastLoginTime)
                .HasColumnType("datetime")
                .HasColumnName("FLastLoginTime");
            entity.Property(e => e.FloginType)
                .HasMaxLength(20)
                .HasDefaultValue("Local")
                .HasColumnName("FLoginType");
            entity.Property(e => e.FpasswordHash)
                .HasMaxLength(255)
                .HasColumnName("FPasswordHash");
            entity.Property(e => e.FphoneNumber)
                .HasMaxLength(20)
                .HasColumnName("FPhoneNumber");
            entity.Property(e => e.FprofileImageUrl)
                .HasMaxLength(255)
                .HasColumnName("FProfileImageUrl");
            entity.Property(e => e.Fstatus)
                .HasDefaultValue((byte)1)
                .HasColumnName("FStatus");
            entity.Property(e => e.FsuspensionEndTime)
                .HasColumnType("datetime")
                .HasColumnName("FSuspensionEndTime");
            entity.Property(e => e.FsuspensionReason)
                .HasMaxLength(500)
                .HasColumnName("FSuspensionReason");
            entity.Property(e => e.FupdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("FUpdatedAt");

            entity.HasMany(d => d.FuserTypes).WithMany(p => p.Fusers)
                .UsingEntity<Dictionary<string, object>>(
                    "TUserUserType",
                    r => r.HasOne<TUserType>().WithMany()
                        .HasForeignKey("FuserTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tUserUser__FUser__44FF419A"),
                    l => l.HasOne<TUser>().WithMany()
                        .HasForeignKey("FuserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__tUserUser__FUser__440B1D61"),
                    j =>
                    {
                        j.HasKey("FuserId", "FuserTypeId").HasName("PK__tUserUse__1BCD2769AC52F484");
                        j.ToTable("tUserUserTypes");
                        j.IndexerProperty<int>("FuserId").HasColumnName("FUserID");
                        j.IndexerProperty<int>("FuserTypeId").HasColumnName("FUserTypeID");
                    });
        });

        modelBuilder.Entity<TUserNotification>(entity =>
        {
            entity.HasKey(e => e.FNotificationId).HasName("PK__tUserNot__A7E1C6B5BEE83835");

            entity.ToTable("tUserNotifications");

            entity.Property(e => e.FNotificationId).HasColumnName("fNotificationID");
            entity.Property(e => e.FCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fCreatedAt");
            entity.Property(e => e.FIsRead).HasColumnName("fIsRead");
            entity.Property(e => e.FMessage).HasColumnName("fMessage");
            entity.Property(e => e.FNotifyType)
                .HasMaxLength(50)
                .HasColumnName("fNotifyType");
            entity.Property(e => e.FRelatedId).HasColumnName("fRelatedID");
            entity.Property(e => e.FSenderId).HasColumnName("fSenderID");
            entity.Property(e => e.FUserId).HasColumnName("fUserID");

            entity.HasOne(d => d.FSender).WithMany(p => p.TUserNotificationFSenders)
                .HasForeignKey(d => d.FSenderId)
                .HasConstraintName("FK__tUserNoti__fSend__1BC821DD");

            entity.HasOne(d => d.FUser).WithMany(p => p.TUserNotificationFUsers)
                .HasForeignKey(d => d.FUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tUserNoti__fUser__1AD3FDA4");
        });

        modelBuilder.Entity<TUserType>(entity =>
        {
            entity.HasKey(e => e.FuserTypeId).HasName("PK__tUserTyp__E088D42A7C75EA62");

            entity.ToTable("tUserTypes");

            entity.HasIndex(e => e.FuserTypeName, "UQ__tUserTyp__EE267F7ABB5B292F").IsUnique();

            entity.Property(e => e.FuserTypeId).HasColumnName("FUserTypeID");
            entity.Property(e => e.FuserTypeName)
                .HasMaxLength(20)
                .HasColumnName("FUserTypeName");
        });

        modelBuilder.Entity<TVerification>(entity =>
        {
            entity.HasKey(e => e.FverificationId).HasName("PK__tVerific__1D00EA7E1AC2D076");

            entity.ToTable("tVerifications");

            entity.HasIndex(e => e.Ftoken, "UQ__tVerific__B6A2AFD8173C3498").IsUnique();

            entity.Property(e => e.FverificationId).HasColumnName("FVerificationID");
            entity.Property(e => e.FexpirationTime)
                .HasColumnType("datetime")
                .HasColumnName("FExpirationTime");
            entity.Property(e => e.FisUsed)
                .HasDefaultValue(false)
                .HasColumnName("FIsUsed");
            entity.Property(e => e.Ftoken)
                .HasMaxLength(255)
                .HasColumnName("FToken");
            entity.Property(e => e.FtokenSentTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FTokenSentTime");
            entity.Property(e => e.FtokenType)
                .HasMaxLength(20)
                .HasColumnName("FTokenType");
            entity.Property(e => e.FusedTime)
                .HasColumnType("datetime")
                .HasColumnName("FUsedTime");
            entity.Property(e => e.FuserId).HasColumnName("FUserID");

            entity.HasOne(d => d.Fuser).WithMany(p => p.TVerifications)
                .HasForeignKey(d => d.FuserId)
                .HasConstraintName("FK_Verifications_Users");
        });

        modelBuilder.Entity<TWorker>(entity =>
        {
            entity.HasKey(e => e.FuserId).HasName("PK__tWorkers__A5C5AA2B6F584678");

            entity.ToTable("tWorkers");

            entity.Property(e => e.FuserId)
                .ValueGeneratedNever()
                .HasColumnName("FUserID");
            entity.Property(e => e.FaboutMe).HasColumnName("FAboutMe");
            entity.Property(e => e.FcodeName)
                .HasMaxLength(50)
                .HasColumnName("FCodeName");
            entity.Property(e => e.FcompletedTasksCount)
                .HasDefaultValue(0)
                .HasColumnName("FCompletedTasksCount");
            entity.Property(e => e.FcreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FCreatedAt");
            entity.Property(e => e.FexperienceYears).HasColumnName("FExperienceYears");
            entity.Property(e => e.FisDeleted)
                .HasDefaultValue(false)
                .HasColumnName("FIsDeleted");
            entity.Property(e => e.FisVerified)
                .HasDefaultValue(false)
                .HasColumnName("FIsVerified");
            entity.Property(e => e.FprofileDescription)
                .HasMaxLength(3000)
                .HasColumnName("FProfileDescription");
            entity.Property(e => e.Frating).HasColumnName("FRating");
            entity.Property(e => e.FserviceAvailability).HasColumnName("FServiceAvailability");
            entity.Property(e => e.Fskills)
                .HasMaxLength(255)
                .HasColumnName("FSkills");
            entity.Property(e => e.FupdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("FUpdatedAt");
            entity.Property(e => e.FwebsiteUrl)
                .HasMaxLength(500)
                .HasColumnName("FWebsiteURL");

            entity.HasOne(d => d.Fuser).WithOne(p => p.TWorker)
                .HasForeignKey<TWorker>(d => d.FuserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Workers_Users");
        });

        modelBuilder.Entity<TWorkerFollow>(entity =>
        {
            entity.HasKey(e => e.FfollowId).HasName("PK__tWorkerF__A894E154E8A46470");

            entity.ToTable("tWorkerFollows");

            entity.HasIndex(e => new { e.FfollowerId, e.FworkerUserId }, "UQ_WorkerFollows").IsUnique();

            entity.Property(e => e.FfollowId).HasColumnName("FFollowID");
            entity.Property(e => e.FcreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FCreatedAt");
            entity.Property(e => e.FfollowerId).HasColumnName("FFollowerID");
            entity.Property(e => e.FworkerUserId).HasColumnName("FWorkerUserID");

            entity.HasOne(d => d.Ffollower).WithMany(p => p.TWorkerFollowFfollowers)
                .HasForeignKey(d => d.FfollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkerFollows_Follower");

            entity.HasOne(d => d.FworkerUser).WithMany(p => p.TWorkerFollowFworkerUsers)
                .HasForeignKey(d => d.FworkerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkerFollows_Worker");
        });

        modelBuilder.Entity<Ttransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__TTransac__55433A4B9332FD86");

            entity.ToTable("TTransaction");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.FinishTime).HasColumnType("datetime");
            entity.Property(e => e.PostUserId).HasColumnName("PostUserID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TaskId).HasColumnName("Task_ID");
            entity.Property(e => e.WorkUserId).HasColumnName("WorkUserID");

            entity.HasOne(d => d.PostUser).WithMany(p => p.TtransactionPostUsers)
                .HasForeignKey(d => d.PostUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostUser");

            entity.HasOne(d => d.Task).WithMany(p => p.Ttransactions)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task");

            entity.HasOne(d => d.WorkUser).WithMany(p => p.TtransactionWorkUsers)
                .HasForeignKey(d => d.WorkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkUser");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
