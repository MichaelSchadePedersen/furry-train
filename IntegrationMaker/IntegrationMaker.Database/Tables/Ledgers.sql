CREATE TABLE [dbo].[Ledgers]
(
	[Id] BIGINT Identity(1,1) NOT NULL,
	[CompanyId] BIGINT NOT NULL,
	[Name] Varchar(500) NOT NULL,
	[ExternalNumber] Varchar(500) NOT NULL,
	LedgerTypeId INT NOT NULL,
    IsBalance BIT NOT NULL,
    IsAccessible BIT NOT NULL,
	[FromAccount] BIGINT,
	[ToAccount] BIGINT,
	SumAccounts nvarchar(500)
    CONSTRAINT [PK_Ledger] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Ledger_Company] FOREIGN KEY (CompanyId) REFERENCES [Company]([Id]))