CREATE TABLE [Membership].[RegisteredUser] (
    [UserID]                                  INT            NOT NULL, -- Primary key for RegisteredUser records. A foreign key to [Membership].[User].[UserID]
    [Email]                                   [dbo].[Email]  NOT NULL, -- Email address of a user.
    [EmailHash]                               AS                       -- MD5 hash string of an email address.
                                              (lower(CONVERT([varchar](32),hashbytes('MD5',CONVERT([varchar],lower(rtrim(ltrim([Email]))),(0))),(2)))) PERSISTED,
    [PasswordHash]                            BINARY (32)    NOT NULL, -- Password hash bytes.
    [PasswordSalt]                            BINARY (32)    NOT NULL, -- Password salt bytes.
    [PasswordQuestion]                        NVARCHAR (250) NULL,     -- A secret question which is used to reset user's password.
    [PasswordAnswer]                          NVARCHAR (100) NULL,     -- User's answer to the secret question.
    [IsActive]                                [dbo].[Flag]   NOT NULL, -- (0) - account is inactive, (1) - account is active
    [IsLockedOut]                             [dbo].[Flag]   NOT NULL, -- (0) - account is not locked out, (1) - account is locked out
    [CreatedDate]                             DATETIME       NOT NULL, -- The UTC date and time when user's account was created.
    [LastLoginDate]                           DATETIME       NULL,     -- The UTC date and time when user logged in into his account last time.
    [LastPasswordChangedDate]                 DATETIME       NULL,     -- The UTC date and time when the password was changed last time.
    [LastLockoutDate]                         DATETIME       NULL,     -- The UTC date and time when user's account was locked out last time.
    [FailedPasswordAttemptCount]              INT            NULL,
    [FailedPasswordAttemptWindowStart]        DATETIME       NULL,
    [FailedPasswordAnswerAttemptCount]        INT            NULL,
    [FailedPasswordAnswerAttempltWindowStart] DATETIME       NULL,
    [Comment]                                 NVARCHAR(1000) NULL,
    CONSTRAINT [PK_Member_UserID] PRIMARY KEY CLUSTERED ([UserID] ASC)
);

