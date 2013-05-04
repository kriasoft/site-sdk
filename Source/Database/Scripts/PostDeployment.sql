---------------------------------------------------------------------------------------------------
-- Import ReferenceData\User.csv
---------------------------------------------------------------------------------------------------

IF OBJECT_ID('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp;
GO

SELECT TOP(0) [UserID], [UserName], [DisplayName], [Email], CAST([PasswordHash] AS nvarchar(100)) AS [PasswordHash], CAST([PasswordSalt] AS nvarchar(100)) AS [PasswordSalt], CAST([IsApproved] AS NVARCHAR(100)) AS [IsApproved], [CreatedDate], [LastLoginDate], [LastActivityDate] INTO #Temp FROM [Membership].[User];

BULK INSERT #Temp FROM '$(ReferenceDataDir)User.csv'
WITH (FIRSTROW = 2, CODEPAGE = 'RAW', DATAFILETYPE = 'char', FIELDTERMINATOR = ' | ', ROWTERMINATOR = '\n', KEEPNULLS, KEEPIDENTITY);
GO

SET IDENTITY_INSERT [Membership].[User] ON;
GO

MERGE INTO [Membership].[User] AS Target
USING (SELECT [UserID], [UserName], [DisplayName], [Email], CONVERT(VARBINARY(24), [PasswordHash], 1) AS [PasswordHash], CONVERT(VARBINARY(24), [PasswordSalt], 1) AS [PasswordSalt], CAST([IsApproved] AS BIT) AS [IsApproved], (0) AS [IsLockedOut], [CreatedDate], [LastLoginDate], [LastActivityDate], (0) AS [FailedPasswordAttemptCount], (0) AS [FailedPasswordAnswerAttemptCount] FROM #Temp) AS Source
ON Target.[UserID] = Source.[UserID]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET [UserName] = RTRIM(Source.[UserName]), [DisplayName] = Source.[DisplayName], [Email] = Source.[Email], [PasswordHash] = Source.[PasswordHash], [PasswordSalt] = Source.[PasswordSalt], [IsApproved] = Source.[IsApproved], [IsLockedOut] = Source.[IsLockedOut], [CreatedDate] = Source.[CreatedDate], [LastLoginDate] = Source.[LastLoginDate], [LastActivityDate] = Source.[LastActivityDate], [FailedPasswordAttemptCount] = Source.[FailedPasswordAttemptCount], [FailedPasswordAnswerAttemptCount] = Source.[FailedPasswordAnswerAttemptCount]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([UserID], [UserName], [DisplayName], [Email], [PasswordHash], [PasswordSalt], [IsApproved], [IsLockedOut], [CreatedDate], [LastLoginDate], [LastActivityDate], [FailedPasswordAttemptCount], [FailedPasswordAnswerAttemptCount])
VALUES ([UserID], RTRIM([UserName]), RTRIM([DisplayName]), RTRIM([Email]), [PasswordHash], [PasswordSalt], [IsApproved], [IsLockedOut], [CreatedDate], [LastLoginDate], [LastActivityDate], [FailedPasswordAttemptCount], [FailedPasswordAnswerAttemptCount]);
GO

SET IDENTITY_INSERT [Membership].[User] OFF;
GO

---------------------------------------------------------------------------------------------------
-- Import ReferenceData\UserRole.csv
---------------------------------------------------------------------------------------------------

IF OBJECT_ID('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp;
GO

SELECT TOP(0) CAST([RoleID] AS TINYINT) AS [RoleID], [RoleName] INTO #Temp FROM [Membership].[UserRole];

BULK INSERT #Temp FROM '$(ReferenceDataDir)UserRole.csv'
WITH (FIRSTROW = 2, FIELDTERMINATOR = ' | ', ROWTERMINATOR = '\n', KEEPNULLS, KEEPIDENTITY);
GO

MERGE INTO [Membership].[UserRole] AS Target
USING (SELECT [RoleID], [RoleName] FROM #Temp) AS Source
ON Target.[RoleID] = Source.[RoleID]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET [RoleName] = Source.[RoleName]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([RoleID], [RoleName])
VALUES ([RoleID], [RoleName])
-- delete rows that are in the target but not the source
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
GO

---------------------------------------------------------------------------------------------------
-- Import ReferenceData\UserUserRole.csv
---------------------------------------------------------------------------------------------------

IF OBJECT_ID('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp;
GO

SELECT TOP(0) [UserID], [RoleID] INTO #Temp FROM [Membership].[UserUserRole];

BULK INSERT #Temp FROM '$(ReferenceDataDir)UserUserRole.csv'
WITH (FIRSTROW = 2, FIELDTERMINATOR = ' | ', ROWTERMINATOR = '\n', KEEPNULLS);
GO

MERGE INTO [Membership].[UserUserRole] AS Target
USING (SELECT [UserID], [RoleID] FROM #Temp) AS Source
ON Target.[UserID] = Source.[UserID] AND Target.[RoleID] = Source.[RoleID]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([UserID], [RoleID])
VALUES ([UserID], [RoleID]);
GO

IF OBJECT_ID('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp;
GO
