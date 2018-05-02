CREATE TABLE [dbo].[Integration]
(
	[Id] BIGINT Identity(1,1) NOT NULL,
	[IntegrationTypeId] BIGINT NOT NULL,
	[CompanyId] BIGINT NOT NULL,
	[CreatedUTC] DATETIME NOT NULL,
	[LastModifiedUTC] DATETIME NULL,
	[IsRunning] BIT NOT NULL,
	[ErrorCode] int NOT NULL,
	[LastIncrementalDataImortUTC] DATETIME NULL,
	[LastFullDataImortUTC] DATETIME NULL,
	[Name] Varchar(500) NOT NULL,
	[ExternalNumber] Varchar(500) NOT NULL,
    CONSTRAINT [FK_Integration_IntegrationType] FOREIGN KEY (IntegrationTypeId) REFERENCES IntegrationType(Id),
    CONSTRAINT [PK_Integration] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Integration_Company] FOREIGN KEY (CompanyId) REFERENCES [Company]([Id])
)
