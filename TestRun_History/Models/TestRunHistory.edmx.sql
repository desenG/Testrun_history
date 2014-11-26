
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/15/2013 08:22:10
-- Generated from EDMX file: D:\TFS\IRTS\Tools\TestRun_History\TestRun_History\Models\TestRunHistory.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [TestRun_History];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Steps_TestRun_Data]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Steps] DROP CONSTRAINT [FK_Steps_TestRun_Data];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Clause]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clause];
GO
IF OBJECT_ID(N'[dbo].[Query]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Query];
GO
IF OBJECT_ID(N'[dbo].[Steps]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Steps];
GO
IF OBJECT_ID(N'[dbo].[TestRun_Data]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TestRun_Data];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Queries'
CREATE TABLE [dbo].[Queries] (
    [QueryID] int IDENTITY(1,1) NOT NULL,
    [Name] nchar(4000)  NULL
);
GO

-- Creating table 'Steps'
CREATE TABLE [dbo].[Steps] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [TR_Num] int  NOT NULL,
    [StepDetail] nvarchar(max)  NULL,
    [ExpectedResult] nvarchar(max)  NULL,
    [Checked] nvarchar(50)  NULL,
    [Comments] nvarchar(max)  NULL,
    [TestRun_DataID] int  NOT NULL
);
GO

-- Creating table 'TestRun_Data'
CREATE TABLE [dbo].[TestRun_Data] (
    [TR_Num] int  NOT NULL,
    [TC_Num] int  NULL,
    [T_Summary] nvarchar(max)  NULL,
    [T_Type] nvarchar(max)  NULL,
    [T_Main_Component] nvarchar(max)  NULL,
    [T_Feature_ID] nvarchar(max)  NULL,
    [env_IDEA_Encoding] nvarchar(max)  NULL,
    [env_OS] nvarchar(max)  NULL,
    [env_User_Type] nvarchar(max)  NULL,
    [T_Status] nvarchar(max)  NULL,
    [TR_Created_By] nvarchar(max)  NULL,
    [TR_Date_Created] datetime  NULL,
    [TR_Last_Mod_By] nvarchar(max)  NULL,
    [TR_Date_Last_Mod] datetime  NULL,
    [product] nvarchar(50)  NULL,
    [isAutomated] nchar(10)  NULL,
    [Estimated_Time] int  NULL,
    [T_Variants] nvarchar(max)  NULL,
    [ProblemStatement] nvarchar(max)  NULL,
    [Build] nvarchar(max)  NULL,
    [PartialFailNotes] nvarchar(max)  NULL,
    [Folders] nvarchar(max)  NULL,
    [Language] nvarchar(max)  NULL,
    [Flavour] nvarchar(max)  NULL,
    [Task] nvarchar(max)  NULL,
    [TestRun_DataID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [QueryID] in table 'Queries'
ALTER TABLE [dbo].[Queries]
ADD CONSTRAINT [PK_Queries]
    PRIMARY KEY CLUSTERED ([QueryID] ASC);
GO

-- Creating primary key on [ID] in table 'Steps'
ALTER TABLE [dbo].[Steps]
ADD CONSTRAINT [PK_Steps]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [TestRun_DataID] in table 'TestRun_Data'
ALTER TABLE [dbo].[TestRun_Data]
ADD CONSTRAINT [PK_TestRun_Data]
    PRIMARY KEY CLUSTERED ([TestRun_DataID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TestRun_DataID] in table 'Steps'
ALTER TABLE [dbo].[Steps]
ADD CONSTRAINT [FK_Steps_TestRun_Data]
    FOREIGN KEY ([TestRun_DataID])
    REFERENCES [dbo].[TestRun_Data]
        ([TestRun_DataID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Steps_TestRun_Data'
CREATE INDEX [IX_FK_Steps_TestRun_Data]
ON [dbo].[Steps]
    ([TestRun_DataID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------