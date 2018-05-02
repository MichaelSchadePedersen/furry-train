﻿/*
Deployment script for furrytrain

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "furrytrain"
:setvar DefaultFilePrefix "furrytrain"
:setvar DefaultDataPath ""
:setvar DefaultLogPath ""

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
IF EXISTS (SELECT 1
           FROM   [sys].[databases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [sys].[databases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [sys].[databases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET QUERY_STORE (CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), MAX_STORAGE_SIZE_MB = 100) 
            WITH ROLLBACK IMMEDIATE;
    END


GO
PRINT N'Creating [dbo].[Company]...';


GO
CREATE TABLE [dbo].[Company] (
    [Id]   BIGINT          NOT NULL,
    [Name] NVARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[CompanyUser]...';


GO
CREATE TABLE [dbo].[CompanyUser] (
    [Id]        BIGINT          NOT NULL,
    [Name]      NVARCHAR (1000) NOT NULL,
    [CompanyId] BIGINT          NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[EconomicAPP]...';


GO
CREATE TABLE [dbo].[EconomicAPP] (
    [Id]             INT            NOT NULL,
    [Name]           NVARCHAR (500) NOT NULL,
    [AppSecretToken] NVARCHAR (500) NOT NULL,
    [RequestURL]     NVARCHAR (500) NOT NULL,
    [AppPublicToken] NVARCHAR (500) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Integration]...';


GO
CREATE TABLE [dbo].[Integration] (
    [Id]                          BIGINT        IDENTITY (1, 1) NOT NULL,
    [IntegrationTypeId]           BIGINT        NOT NULL,
    [CompanyId]                   BIGINT        NOT NULL,
    [CreatedUTC]                  DATETIME      NOT NULL,
    [LastModifiedUTC]             DATETIME      NULL,
    [IsRunning]                   BIT           NOT NULL,
    [ErrorCode]                   INT           NOT NULL,
    [LastIncrementalDataImortUTC] DATETIME      NULL,
    [LastFullDataImortUTC]        DATETIME      NULL,
    [Name]                        VARCHAR (500) NOT NULL,
    [ExternalNumber]              VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_Integration] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[IntegrationCredential]...';


GO
CREATE TABLE [dbo].[IntegrationCredential] (
    [Id]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [AccountIdentifier] NVARCHAR (500) NULL,
    [Username]          NVARCHAR (500) NULL,
    [IntegrationId]     BIGINT         NOT NULL,
    [Password]          NVARCHAR (500) NULL,
    [Token]             NVARCHAR (100) NULL,
    [CratedUTC]         DATETIME       NOT NULL,
    [LastModifiedUTC]   DATETIME       NULL,
    CONSTRAINT [PK_IntegrationCredential] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[IntegrationErrorCode]...';


GO
CREATE TABLE [dbo].[IntegrationErrorCode] (
    [Id]                      BIGINT         IDENTITY (1, 1) NOT NULL,
    [Number]                  BIGINT         NOT NULL,
    [Name]                    NVARCHAR (500) NOT NULL,
    [UserfriendlyDescreption] NVARCHAR (MAX) NOT NULL,
    [TechnicalDescription]    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_IntegrationErrorCodes] PRIMARY KEY CLUSTERED ([Number] ASC)
);


GO
PRINT N'Creating [dbo].[IntegrationLog]...';


GO
CREATE TABLE [dbo].[IntegrationLog] (
    [Id]                     BIGINT IDENTITY (1, 1) NOT NULL,
    [IntegrationId]          BIGINT NOT NULL,
    [IntegrationErrorNumber] BIGINT NULL
);


GO
PRINT N'Creating [dbo].[IntegrationType]...';


GO
CREATE TABLE [dbo].[IntegrationType] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (500) NOT NULL,
    [CreatedUTC] DATETIME       NOT NULL,
    CONSTRAINT [PK_IntegrationTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Job]...';


GO
CREATE TABLE [dbo].[Job] (
    [Id]                      BIGINT         IDENTITY (1, 1) NOT NULL,
    [TargetId]                BIGINT         NOT NULL,
    [JobTypeId]               BIGINT         NOT NULL,
    [JobStateId]              BIGINT         NOT NULL,
    [UserId]                  BIGINT         NULL,
    [NotifyWhenComplete]      BIT            NOT NULL,
    [ErpIntegrationErrorCode] INT            NULL,
    [CreatedBy]               NVARCHAR (MAX) NULL,
    [UTCCreated]              DATETIME       NOT NULL,
    [UTCStarted]              DATETIME       NULL,
    [UTCFinished]             DATETIME       NULL,
    [IsMonitored]             BIT            NOT NULL,
    CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[JobLog]...';


GO
CREATE TABLE [dbo].[JobLog] (
    [Id]             BIGINT         NOT NULL,
    [JobId]          BIGINT         NOT NULL,
    [Information]    NVARCHAR (MAX) NOT NULL,
    [IsWarning]      BIT            NOT NULL,
    [IsError]        BIT            NOT NULL,
    [DisplayForUser] BIT            NOT NULL,
    [UTCCreated]     DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[JobState]...';


GO
CREATE TABLE [dbo].[JobState] (
    [Id]   BIGINT         NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[JobTransferDetail]...';


GO
CREATE TABLE [dbo].[JobTransferDetail] (
    [Id]              BIGINT         NOT NULL,
    [JobId]           BIGINT         NOT NULL,
    [TableName]       NVARCHAR (MAX) NOT NULL,
    [RowsTransferred] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[JobType]...';


GO
CREATE TABLE [dbo].[JobType] (
    [Id]         BIGINT         NOT NULL,
    [Name]       NVARCHAR (MAX) NULL,
    [TargetType] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Ledgers]...';


GO
CREATE TABLE [dbo].[Ledgers] (
    [Id]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [CompanyId]      BIGINT         NOT NULL,
    [Name]           VARCHAR (500)  NOT NULL,
    [ExternalNumber] VARCHAR (500)  NOT NULL,
    [LedgerTypeId]   INT            NOT NULL,
    [IsBalance]      BIT            NOT NULL,
    [IsAccessible]   BIT            NOT NULL,
    [FromAccount]    BIGINT         NULL,
    [ToAccount]      BIGINT         NULL,
    [SumAccounts]    NVARCHAR (500) NULL,
    CONSTRAINT [PK_Ledger] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating unnamed constraint on [dbo].[Job]...';


GO
ALTER TABLE [dbo].[Job]
    ADD DEFAULT ((0)) FOR [IsMonitored];


GO
PRINT N'Creating [dbo].[FK_CompanyUser_Company]...';


GO
ALTER TABLE [dbo].[CompanyUser] WITH NOCHECK
    ADD CONSTRAINT [FK_CompanyUser_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id]);


GO
PRINT N'Creating [dbo].[FK_Integration_IntegrationType]...';


GO
ALTER TABLE [dbo].[Integration] WITH NOCHECK
    ADD CONSTRAINT [FK_Integration_IntegrationType] FOREIGN KEY ([IntegrationTypeId]) REFERENCES [dbo].[IntegrationType] ([Id]);


GO
PRINT N'Creating [dbo].[FK_Integration_Company]...';


GO
ALTER TABLE [dbo].[Integration] WITH NOCHECK
    ADD CONSTRAINT [FK_Integration_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id]);


GO
PRINT N'Creating [dbo].[FK_IntegrationCredential_Integration]...';


GO
ALTER TABLE [dbo].[IntegrationCredential] WITH NOCHECK
    ADD CONSTRAINT [FK_IntegrationCredential_Integration] FOREIGN KEY ([IntegrationId]) REFERENCES [dbo].[Integration] ([Id]);


GO
PRINT N'Creating [dbo].[FK_IntegrationLog_Integration]...';


GO
ALTER TABLE [dbo].[IntegrationLog] WITH NOCHECK
    ADD CONSTRAINT [FK_IntegrationLog_Integration] FOREIGN KEY ([IntegrationId]) REFERENCES [dbo].[Integration] ([Id]);


GO
PRINT N'Creating [dbo].[FK_IntegrationLog_IntegrationErrorCode]...';


GO
ALTER TABLE [dbo].[IntegrationLog] WITH NOCHECK
    ADD CONSTRAINT [FK_IntegrationLog_IntegrationErrorCode] FOREIGN KEY ([IntegrationErrorNumber]) REFERENCES [dbo].[IntegrationErrorCode] ([Number]);


GO
PRINT N'Creating [dbo].[FK_JobLog_Job]...';


GO
ALTER TABLE [dbo].[JobLog] WITH NOCHECK
    ADD CONSTRAINT [FK_JobLog_Job] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Job] ([Id]);


GO
PRINT N'Creating [dbo].[FK_JobTransferDetail_Job]...';


GO
ALTER TABLE [dbo].[JobTransferDetail] WITH NOCHECK
    ADD CONSTRAINT [FK_JobTransferDetail_Job] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Job] ([Id]);


GO
PRINT N'Creating [dbo].[FK_Ledger_Company]...';


GO
ALTER TABLE [dbo].[Ledgers] WITH NOCHECK
    ADD CONSTRAINT [FK_Ledger_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id]);


GO
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
INSERT INTO [dbo].[IntegrationType]
           (Name,CreatedUTC)
     VALUES
           ('economic','2018-03-27')
INSERT INTO [dbo].[IntegrationType]
           (Name,CreatedUTC)
     VALUES
           ('Dynamic365BusinessCentral','2018-05-2')


INSERT INTO [dbo].[EconomicAPP]
(Name, AppSecretToken,RequestURL,AppPublicToken)
VALUES ('FurryTrain','BziA4Tls2Bgp4STB3lDJH0ReTtoUwK8xMCHC1AXMwI81','https://secure.e-conomic.com/secure/api1/requestaccess.aspx?appPublicToken=aM4bqkvcBR4HSTvgdRKnevJs8Y05lMLyKFivF90Qlto1','aM4bqkvcBR4HSTvgdRKnevJs8Y05lMLyKFivF90Qlto1')
GO

GO
