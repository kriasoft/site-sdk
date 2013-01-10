---------------------------------------------------------------------------------------------------
-- Import ReferenceData\UserRole.csv
---------------------------------------------------------------------------------------------------

IF OBJECT_ID('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp;

SELECT TOP(0) CAST([RoleID] AS TINYINT) AS [RoleID], [Name] INTO #Temp FROM [Membership].[UserRole];

BULK INSERT #Temp FROM '$(ReferenceDataDir)UserRole.csv'
WITH (FIRSTROW = 2, FIELDTERMINATOR = ' | ', ROWTERMINATOR = '\n', KEEPNULLS);
GO

MERGE INTO [Membership].[UserRole] AS Target
USING (SELECT [RoleID], [Name] FROM #Temp) AS Source
ON Target.[RoleID] = Source.[RoleID]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET [Name] = Source.[Name]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([RoleID], [Name])
VALUES ([RoleID], [Name]);
-- delete rows that are in the target but not the source
-- WHEN NOT MATCHED BY SOURCE THEN 
-- DELETE;
GO

IF OBJECT_ID('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp;
GO
