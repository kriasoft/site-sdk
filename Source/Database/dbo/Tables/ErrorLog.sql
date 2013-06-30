-- Audit table tracking errors in the the database that are caught by the CATCH block of a TRY...CATCH construct.
-- Data is inserted by stored procedure dbo.uspLogError when it is executed from inside the CATCH block of a TRY...CATCH construct.
CREATE TABLE [dbo].[ErrorLog] (
    [ErrorLogID]     INT             IDENTITY (1, 1) NOT NULL, -- Primary key for ErrorLog records.
    [ErrorTime]      DATETIME        NOT NULL                  -- The date and time at which the error occurred.
                                     CONSTRAINT [DF_ErrorLog_ErrorTime] DEFAULT (getdate()), -- Default constraint value of GETDATE()
    [UserName]       [sysname]       NOT NULL,                 -- The user who executed the batch in which the error occurred.
    [ErrorNumber]    INT             NOT NULL,                 -- The error number of the error that occurred.
    [ErrorSeverity]  INT             NULL,                     -- The severity of the error that occurred.
    [ErrorState]     INT             NULL,                     -- The state number of the error that occurred.
    [ErrorProcedure] NVARCHAR (126)  NULL,                     -- The name of the stored procedure or trigger where the error occurred.
    [ErrorLine]      INT             NULL,                     -- The line number at which the error occurred.
    [ErrorMessage]   NVARCHAR (4000) NOT NULL,                 -- The message text of the error that occurred
    CONSTRAINT [PK_ErrorLog_ErrorLogID] PRIMARY KEY CLUSTERED ([ErrorLogID] ASC) -- Primary key (clustered) constraint
);
