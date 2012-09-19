CREATE TABLE [Membership].[User] (
    [UserID]           INT            IDENTITY (1, 1) NOT NULL, -- Primary key for User records.
    [UserName]         [dbo].[Name]   NOT NULL,                 -- Login name of the user.
    [PasswordHash]     VARBINARY (32) NULL,                     -- Password hash bytes
    [PasswordSalt]     VARBINARY (16) NULL,                     -- Password salt bytes
    [Email]            [dbo].[Email]  NOT NULL,                 -- Email address of a user.
    [EmailHash]        AS                                       -- Email MD5 hash string.
                                      (lower(CONVERT([varchar](32),hashbytes('MD5',CONVERT([varchar],lower(rtrim(ltrim([Email]))),(0))),(2)))) PERSISTED,
    [DisplayName]      [dbo].[Name]   NULL,                     -- Alternate name of the user.
    [RegistrationDate] DATETIME       CONSTRAINT [DF_User_RegistrationDate] DEFAULT (getutcdate()) NOT NULL,
    [LastLoginDate]    DATETIME       NULL,
    [LastActivityDate] DATETIME       NULL,
    CONSTRAINT [PK_User_UserID] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [UK_User_UserName] UNIQUE NONCLUSTERED ([UserName] ASC)
);

