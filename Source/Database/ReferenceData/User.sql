SET IDENTITY_INSERT [Membership].[User] ON;
GO

MERGE INTO [Membership].[User] AS Target
USING (VALUES

    (1, N'admin', N'Administrator', N'admin@example.com', 0x77D665A1DFC9F32FC6D5E09E755670CCEDA834D0FF4CEFAB, 0xCAFD5B1D89620DCD1E34FE9C1BFF104256143B9FA580B9F3, 1, 0, N'2013-01-01 00:00:00', N'2013-01-01 00:00:00', N'2013-01-01 00:00:00', 0, 0)

) AS Source (UserID, UserName, DisplayName, Email, PasswordHash, PasswordSalt, IsApproved, IsLockedOut, CreatedDate, LastLoginDate, LastActivityDate, FailedPasswordAttemptCount, FailedPasswordAnswerAttemptCount)
ON Target.[UserID] = Source.[UserID]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET [UserName] = Source.[UserName], [DisplayName] = Source.[DisplayName], [Email] = Source.[Email], [PasswordHash] = Source.[PasswordHash], [PasswordSalt] = Source.[PasswordSalt], [IsApproved] = Source.[IsApproved], [IsLockedOut] = Source.[IsLockedOut], [CreatedDate] = Source.[CreatedDate], [LastLoginDate] = Source.[LastLoginDate], [LastActivityDate] = Source.[LastActivityDate], [FailedPasswordAttemptCount] = Source.[FailedPasswordAttemptCount], [FailedPasswordAnswerAttemptCount] = Source.[FailedPasswordAnswerAttemptCount]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([UserID], [UserName], [DisplayName], [Email], [PasswordHash], [PasswordSalt], [IsApproved], [IsLockedOut], [CreatedDate], [LastLoginDate], [LastActivityDate], [FailedPasswordAttemptCount], [FailedPasswordAnswerAttemptCount])
VALUES ([UserID], [UserName], [DisplayName], [Email], [PasswordHash], [PasswordSalt], [IsApproved], [IsLockedOut], [CreatedDate], [LastLoginDate], [LastActivityDate], [FailedPasswordAttemptCount], [FailedPasswordAnswerAttemptCount]);
GO

SET IDENTITY_INSERT [Membership].[User] OFF;
GO