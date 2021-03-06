/****** Object:  FullTextCatalog [FTMONUCPersonnelInfo]    Script Date: 01/28/2013 17:38:41 ******/
CREATE FULLTEXT CATALOG [FTMONUCPersonnelInfo]WITH ACCENT_SENSITIVITY = ON
AUTHORIZATION [dbo]
GO
/****** Object:  Table [dbo].[VWUNADDRESSBooKEmail]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VWUNADDRESSBooKEmail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name1] [varchar](255) NULL,
	[name] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_VWUNADDRESSBooKEmail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[trunkingNO]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[trunkingNO](
	[TetraAddress] [float] NULL,
	[Label] [nvarchar](255) NULL,
	[Comment] [nvarchar](255) NULL,
	[unid] [nvarchar](255) NULL,
	[F5] [nvarchar](255) NULL,
	[TrunkingNO_PK] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_trunkingNO] PRIMARY KEY CLUSTERED 
(
	[TrunkingNO_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tempCounter]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tempCounter](
	[staff_info_hits] [int] NULL,
	[Counter_PK] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tempCounter] PRIMARY KEY CLUSTERED 
(
	[Counter_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblWebSiteContent]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblWebSiteContent](
	[Page] [varchar](50) NULL,
	[Content] [varchar](5000) NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblWebSiteContent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblVHFRadio]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblVHFRadio](
	[VHFRadio_PK] [int] IDENTITY(1,1) NOT NULL,
	[RadioID] [int] NOT NULL,
	[IDTelNo] [varchar](20) NULL,
	[DateIssue] [datetime] NULL,
	[BarCode] [varchar](50) NULL,
	[OptionBoard] [varchar](100) NULL,
	[CallSign] [varchar](25) NULL,
	[Remarks] [varchar](200) NULL,
 CONSTRAINT [PK_tblVHFRadio] PRIMARY KEY CLUSTERED 
(
	[VHFRadio_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblVehicleType]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblVehicleType](
	[VehicleType_PK] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](500) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblVehicleType] PRIMARY KEY CLUSTERED 
(
	[VehicleType_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblUserHelpDesk]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblUserHelpDesk](
	[HelpdeskID] [char](10) NULL,
	[Pass] [nvarchar](50) NULL,
	[UserHelpDesk_PK] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblUserHelpDesk] PRIMARY KEY CLUSTERED 
(
	[UserHelpDesk_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTrunkingRadio]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTrunkingRadio](
	[TrunkingRadio_PK] [int] IDENTITY(1,1) NOT NULL,
	[RadioID] [int] NOT NULL,
	[DateIssue] [datetime] NULL,
	[BarCode] [varchar](50) NULL,
	[OptionBoard] [varchar](100) NULL,
	[CallSign] [varchar](25) NULL,
	[Remarks] [varchar](200) NULL,
 CONSTRAINT [PK_tblTrunkingRadio] PRIMARY KEY CLUSTERED 
(
	[TrunkingRadio_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblToDelete]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblToDelete](
	[ID_Card_Number] [nvarchar](max) NULL,
	[Mail_Address] [nvarchar](max) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblTemp]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblTemp](
	[ISSDATE] [smalldatetime] NULL,
	[EXPDATE] [smalldatetime] NULL,
	[FLD11] [nvarchar](25) NULL,
	[FLD1] [nvarchar](25) NULL,
	[Temp_PK] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblTemp] PRIMARY KEY CLUSTERED 
(
	[Temp_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblTelDirStats]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTelDirStats](
	[IdtelNo] [varchar](20) NULL,
	[ExtNo] [bigint] NULL,
	[Email] [bigint] NULL,
	[orgsection] [bigint] NULL,
	[fullname] [bigint] NULL,
	[location] [bigint] NULL,
	[dt] [varchar](50) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblSections]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblSections](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Organization] [nvarchar](255) NULL,
	[HeadOfOrg] [nvarchar](255) NULL,
	[AdminOffice] [nvarchar](255) NULL,
	[Orgsection] [nvarchar](255) NULL,
	[Unit] [nvarchar](255) NULL,
	[Subunit] [nvarchar](255) NULL,
	[SectionShort] [nvarchar](255) NULL,
	[SortValue] [char](10) NULL,
 CONSTRAINT [PK_tblSections ] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/*
CREATE FULLTEXT INDEX ON [dbo].[tblSections](
[AdminOffice] LANGUAGE [English], 
[HeadOfOrg] LANGUAGE [English], 
[Organization] LANGUAGE [English], 
[Orgsection] LANGUAGE [English], 
[SectionShort] LANGUAGE [English], 
[Subunit] LANGUAGE [English], 
[Unit] LANGUAGE [English])
KEY INDEX [PK_tblSections ]ON ([ftsearchSection], FILEGROUP [ftfg_ftsearchSection])
WITH (CHANGE_TRACKING = OFF, STOPLIST = SYSTEM)
GO
*/
/****** Object:  Table [dbo].[tblSectionHierachy]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblSectionHierachy](
	[LookupKey] [varchar](100) NULL,
	[LookupValue] [varchar](100) NULL,
	[Level_] [varchar](500) NULL,
	[ID] [varchar](500) NULL,
	[SectionHierarchy_PK] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblSectionHierachy] PRIMARY KEY CLUSTERED 
(
	[SectionHierarchy_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRolePermission]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRolePermission](
	[RoleID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
	[RolePermission_PK] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblRolePermission] PRIMARY KEY CLUSTERED 
(
	[RolePermission_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblRole]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblRole](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](50) NOT NULL,
	[ApplicationID] [int] NULL,
 CONSTRAINT [PK_tblRole] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRegionLocation]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblRegionLocation](
	[cityID] [bigint] IDENTITY(1,1) NOT NULL,
	[Cities] [varchar](1000) NULL,
	[regid] [bigint] NULL,
 CONSTRAINT [PK_tblRegionLocation] PRIMARY KEY CLUSTERED 
(
	[cityID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblregion]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblregion](
	[regid] [bigint] IDENTITY(1,1) NOT NULL,
	[region] [varchar](500) NULL,
 CONSTRAINT [PK_tblregion] PRIMARY KEY CLUSTERED 
(
	[regid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRank]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblRank](
	[Rank_PK] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblRank] PRIMARY KEY CLUSTERED 
(
	[Rank_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblPrefixExt]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblPrefixExt](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[location] [varchar](50) NULL,
	[Prefix] [varchar](50) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblPrefixExt] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblPersonnelMasterISRoles]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblPersonnelMasterISRoles](
	[PersonnelMasterISRole_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[ISRole_FK] [int] NULL,
	[AccessibleFields] [varchar](1000) NULL,
 CONSTRAINT [PK_tblPersonnelMasterISRoles] PRIMARY KEY CLUSTERED 
(
	[PersonnelMasterISRole_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblPersonnelMaster]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblPersonnelMaster](
	[IDTelNo] [varchar](20) NOT NULL,
	[IDTelLocation] [varchar](20) NULL,
	[PersonnelUniqueID] [bigint] IDENTITY(1,1) NOT NULL,
	[Password] [varbinary](20) NULL,
	[SecretQuestion] [varchar](50) NULL,
	[SecretAnswer] [varchar](50) NULL,
	[Organization] [varchar](50) NULL,
	[OrgSection] [varchar](50) NULL,
	[Unit] [varchar](50) NULL,
	[Location] [varchar](50) NULL,
	[TeamSiteNo] [varchar](50) NULL,
	[Mission] [varchar](10) NULL,
	[MobilePhoneNos] [varchar](20) NULL,
	[MissionPABXExtention] [varchar](10) NULL,
	[MissionDectPhone] [varchar](20) NULL,
	[MissionRadioCallsign] [varchar](20) NULL,
	[MissionVHFRadio] [varchar](10) NULL,
	[MissionTrunkingNo] [varchar](20) NULL,
	[MissionOfficialCellularNo] [varchar](25) NULL,
	[CountryOriginal] [varchar](50) NULL,
	[Nationality] [varchar](50) NULL,
	[AddressInHomeCountry] [varchar](200) NULL,
	[ContactNosInHomeCountry] [varchar](20) NULL,
	[ContactPersonInHomeCountry] [varchar](50) NULL,
	[MissionAppointmentType] [varchar](30) NULL,
	[PersonnelType] [varchar](30) NULL,
	[PersonnelCategory] [varchar](50) NULL,
	[ArrivalDateInMission] [datetime] NULL,
	[DepartureDate] [datetime] NULL,
	[MissionFunctionalTitle] [varchar](100) NULL,
	[SecurityZoneNo] [varchar](40) NULL,
	[LotusNotesEmailID] [varchar](50) NULL,
	[LotusNotesUserID] [varchar](25) NULL,
	[OtherEmailNo1] [varchar](50) NULL,
	[OtherEmailNo2] [varchar](20) NULL,
	[OtherEmailNo3] [varchar](20) NULL,
	[Languages] [varchar](40) NULL,
	[MilitaryRank] [varchar](20) NULL,
	[BloodType] [varchar](8) NULL,
	[Age] [int] NULL,
	[ParentMission] [varchar](10) NULL,
	[IndexNo] [varchar](10) NULL,
	[Grade] [varchar](50) NULL,
	[PassportNos] [varchar](15) NULL,
	[MailServerName] [varchar](25) NULL,
	[IDTelName] [varchar](50) NULL,
	[Gender] [varchar](10) NULL,
	[Building] [varchar](50) NULL,
	[CheckedOut] [varchar](10) NULL,
	[NOKName] [varchar](50) NULL,
	[NOKAddress] [varchar](200) NULL,
	[NOKContactNo] [varchar](20) NULL,
	[NOKDetails] [varchar](200) NULL,
	[Height] [varchar](10) NULL,
	[Weight] [varchar](10) NULL,
	[UNLPNo] [varchar](15) NULL,
	[EyeColor] [varchar](15) NULL,
	[HairColord] [varchar](50) NULL,
	[Religion] [varchar](100) NULL,
	[Title] [varchar](70) NULL,
	[OtherContactDetails] [varchar](200) NULL,
	[MissionCountry] [varchar](30) NULL,
	[Street] [varchar](50) NULL,
	[UNVehicleNo] [varchar](10) NULL,
	[DutyStation] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[StreetNo] [varchar](200) NULL,
	[BuildingNo] [varchar](200) NULL,
	[DateOfBirth] [datetime] NULL,
	[VisitorClearenceNo] [varchar](20) NULL,
	[ExtensionUntil] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdatedBy] [varchar](50) NULL,
	[LastUpdatedOn] [datetime] NULL,
	[UNIndexNo] [varchar](10) NULL,
	[IsEssential] [bit] NULL,
	[IsWarden] [bit] NULL,
	[IsMilitary] [bit] NULL,
	[IsSectionChief] [bit] NULL,
	[IsVIP] [bit] NULL,
	[IsVisitor] [bit] NULL,
	[IsCritical] [bit] NULL,
	[HairColor] [varchar](50) NULL,
	[IsRegistered] [bit] NOT NULL,
	[StreetNumber] [varchar](300) NULL,
	[AppartmentNo] [varchar](50) NULL,
	[Division] [varchar](500) NULL,
	[SubDivision] [varchar](500) NULL,
	[SubUnit] [varchar](500) NULL,
	[ExpiryDate] [datetime] NULL,
	[Region] [varchar](50) NULL,
	[HID] [int] NULL,
	[DisplayMyMobile] [bit] NOT NULL,
	[PABXPrefix] [varchar](10) NULL,
	[SignatureFilename] [varchar](50) NULL,
	[PhotoFilename] [varchar](50) NULL,
	[importstatus] [char](3) NULL,
	[IdExpirered] [varchar](10) NULL,
	[HideTelDir] [char](3) NULL,
	[Template_FK] [int] NULL,
	[BackTemplate_FK] [int] NULL,
	[EmailUpated] [int] NULL,
	[PasswordUpdated] [int] NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
	[LotusNotesInternetAddress] [varchar](100) NULL,
 CONSTRAINT [PK_tblPersonnelMaster] PRIMARY KEY CLUSTERED 
(
	[PersonnelUniqueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_tblPersonnelMaster] ON [dbo].[tblPersonnelMaster] 
(
	[IDTelNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
CREATE FULLTEXT INDEX ON [dbo].[tblPersonnelMaster](
[FirstName] LANGUAGE [English], 
[IDTelNo] LANGUAGE [English], 
[LastName] LANGUAGE [English], 
[Location] LANGUAGE [English], 
[LotusNotesInternetAddress] LANGUAGE [English], 
[Mission] LANGUAGE [English], 
[MissionFunctionalTitle] LANGUAGE [English], 
[MissionOfficialCellularNo] LANGUAGE [English], 
[MissionPABXExtention] LANGUAGE [English], 
[MissionRadioCallsign] LANGUAGE [English], 
[MissionTrunkingNo] LANGUAGE [English], 
[MissionVHFRadio] LANGUAGE [English], 
[MobilePhoneNos] LANGUAGE [English], 
[Organization] LANGUAGE [English], 
[OrgSection] LANGUAGE [English], 
[PABXPrefix] LANGUAGE [English], 
[Region] LANGUAGE [English], 
[Title] LANGUAGE [English], 
[Unit] LANGUAGE [English])
KEY INDEX [PK_tblPersonnelMaster]ON ([FTMONUCPersonnelInfo], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)
GO
/****** Object:  Table [dbo].[tblPermission]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblPermission](
	[PermissionID] [int] IDENTITY(1,1) NOT NULL,
	[PermissionName] [varchar](50) NOT NULL,
	[PermissionFilename] [varchar](100) NOT NULL,
	[IsMenuIncluded] [bit] NOT NULL,
	[OrderViewed] [int] NOT NULL,
	[Application_FK] [int] NOT NULL,
	[IsSecured] [bit] NOT NULL,
 CONSTRAINT [PK_tblPermission] PRIMARY KEY CLUSTERED 
(
	[PermissionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'This is used when an application is viewed by everybody but there is restricted area on it so we required a mixed mode' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblPermission', @level2type=N'COLUMN',@level2name=N'IsSecured'
GO
/****** Object:  Table [dbo].[tblMPTUsers]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblMPTUsers](
	[UserName] [varchar](20) NOT NULL,
	[UserFullName] [varchar](50) NULL,
	[UserLocation] [varchar](50) NOT NULL,
	[UserSection] [varchar](50) NOT NULL,
	[UserUnit] [varchar](50) NULL,
	[UserEmail] [varchar](50) NOT NULL,
	[UserPhoneExtension] [varchar](30) NOT NULL,
	[RegisteredOn] [datetime] NOT NULL,
	[UserAccessLevel] [tinyint] NOT NULL,
	[CreateEdit] [bit] NOT NULL,
	[emailAlert] [bit] NULL,
	[trafficReport] [bit] NULL,
	[newsArticle] [bit] NULL,
	[searchStaff] [bit] NULL,
	[enterStaff] [bit] NULL,
	[dssDB] [bit] NULL,
	[updateRequest] [bit] NULL,
	[wardenNotice] [bit] NULL,
	[SecFocalPoint] [bit] NOT NULL,
	[MPTUsers_PK] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblMPTUsers] PRIMARY KEY CLUSTERED 
(
	[MPTUsers_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblMessages]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblMessages](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SECTION] [varchar](50) NULL,
	[AUTHOR] [varchar](50) NULL,
	[MESSAGE_TITLE] [varchar](50) NULL,
	[URL] [varchar](50) NULL,
	[RECEPIENT_ID] [varchar](50) NULL,
	[MESSAGE_TYPE] [varchar](50) NULL,
	[DATE_CREATED] [varchar](50) NULL,
	[DELETED] [int] NULL,
	[ROLE] [varchar](50) NULL,
	[LOCATION] [varchar](100) NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_tblMessages] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblMaxIDoftblTDI]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblMaxIDoftblTDI](
	[Maxid] [bigint] NULL,
	[MaxIDoftblTDI_PK] [int] IDENTITY(1,1) NOT NULL,
	[LastUpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_tblMaxIDoftblTDI] PRIMARY KEY CLUSTERED 
(
	[MaxIDoftblTDI_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblLookupsBuildings]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblLookupsBuildings](
	[Country] [varchar](30) NOT NULL,
	[City] [varchar](30) NOT NULL,
	[Sector] [varchar](20) NULL,
	[ZoneText] [varchar](50) NULL,
	[Segment] [int] NULL,
	[Street] [varchar](50) NULL,
	[Building] [varchar](50) NULL,
	[Missions] [varchar](20) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblLookupsBuildings] ON [dbo].[tblLookupsBuildings] 
(
	[Country] ASC,
	[City] ASC,
	[Sector] ASC,
	[ZoneText] ASC,
	[Street] ASC,
	[Building] ASC,
	[Missions] ASC,
	[Segment] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblLookups]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblLookups](
	[LookupKey] [varchar](500) NOT NULL,
	[LookupValue] [varchar](1500) NOT NULL,
	[LookupValueInternal] [varchar](10) NULL,
	[EntityFullname] [varchar](500) NULL,
	[Parent] [varchar](50) NULL,
	[Lookups_PK] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblLookups] PRIMARY KEY CLUSTERED 
(
	[Lookups_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblLeave]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblLeave](
	[Leave_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](25) NOT NULL,
	[DateFrom] [smalldatetime] NOT NULL,
	[DateTo] [smalldatetime] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IdTelNoDuty] [varchar](25) NULL,
 CONSTRAINT [PK_tblLeave] PRIMARY KEY CLUSTERED 
(
	[Leave_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblISRoles]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblISRoles](
	[ISRole_PK] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](50) NOT NULL,
	[URL] [varchar](200) NULL,
	[IsRoleBased] [bit] NOT NULL,
	[IsPopUp] [bit] NOT NULL,
	[Application_FK] [int] NULL,
 CONSTRAINT [PK_tblISRoles] PRIMARY KEY CLUSTERED 
(
	[ISRole_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblImportErrors]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblImportErrors](
	[ErrorLocation] [varchar](15) NOT NULL,
	[ErrorIDNo] [varchar](10) NULL,
	[ErrorText] [varchar](100) NULL,
	[ErrorDate] [datetime] NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblImportErrors] PRIMARY KEY CLUSTERED 
(
	[ErrorLocation] ASC,
	[ErrorDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIDTelTextDataImport]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIDTelTextDataImport](
	[IDTelNo] [varchar](25) NOT NULL,
	[IDTelSysIndexNo] [bigint] NOT NULL,
	[IDTelLocation] [varchar](20) NULL,
	[IDTelDesignTemplate] [int] NULL,
	[IDTelName] [varchar](150) NULL,
	[IDTelDesignation] [varchar](150) NULL,
	[IDTelIssuedDate] [datetime] NULL,
	[IDTelExpiryDate] [datetime] NULL,
	[IDTelDateOfBirth] [datetime] NULL,
	[IDTelBloodGroup] [varchar](25) NULL,
	[IDTelCreatedDate] [datetime] NULL,
	[IDTelLastImported] [datetime] NULL,
	[IDTelLastPrintDate] [datetime] NULL,
	[UniqID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblIDTelTextDataImport] PRIMARY KEY CLUSTERED 
(
	[IDTelNo] ASC,
	[IDTelSysIndexNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY],
 CONSTRAINT [IX_tblIDTelTextDataImport] UNIQUE NONCLUSTERED 
(
	[IDTelNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIdTelTemplateField]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblIdTelTemplateField](
	[IdTelTemplateField_PK] [int] IDENTITY(1,1) NOT NULL,
	[Template_FK] [int] NOT NULL,
	[Field_FK] [int] NOT NULL,
 CONSTRAINT [PK_tblIdTelTemplateField] PRIMARY KEY CLUSTERED 
(
	[IdTelTemplateField_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblIdTelTemplate]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIdTelTemplate](
	[Template_PK] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[IsHorizontal] [bit] NOT NULL,
	[CardFace] [bit] NOT NULL,
	[Application_FK] [int] NOT NULL,
 CONSTRAINT [PK_tblIdTelTemplate] PRIMARY KEY CLUSTERED 
(
	[Template_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIDTelRoles]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIDTelRoles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[IDTelNo] [varchar](20) NULL,
	[RoleName] [varchar](30) NULL,
	[RoleValue] [int] NULL,
	[Section] [varchar](50) NULL,
	[Author] [varchar](10) NULL,
	[Name] [varchar](500) NULL,
	[Location] [varchar](300) NULL,
 CONSTRAINT [PK_tblIDTelRoles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIdTelHistory]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIdTelHistory](
	[History_PK] [int] IDENTITY(1,1) NOT NULL,
	[Application_FK] [int] NOT NULL,
	[DateEdited] [datetime] NOT NULL,
	[EditedBy] [varchar](20) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[Activity] [varchar](200) NULL,
 CONSTRAINT [PK_tblIdTelHistory] PRIMARY KEY CLUSTERED 
(
	[History_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIDTelGraphicDataImport]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIDTelGraphicDataImport](
	[IDTelNo] [varchar](25) NOT NULL,
	[IDTelSysIndexNo] [bigint] NOT NULL,
	[IDTelLocation] [varchar](20) NULL,
	[IDTelPhoto] [image] NULL,
	[IDTelSignature] [image] NULL,
	[IDTelLastImported] [datetime] NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblIDTelGraphicDataImport] PRIMARY KEY CLUSTERED 
(
	[IDTelNo] ASC,
	[IDTelSysIndexNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY],
 CONSTRAINT [IX_tblIDTelGraphicDataImport] UNIQUE NONCLUSTERED 
(
	[IDTelNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIdTelField]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIdTelField](
	[Field_PK] [int] IDENTITY(1,1) NOT NULL,
	[ScreenX] [int] NOT NULL,
	[ScreenY] [int] NOT NULL,
	[Width] [int] NOT NULL,
	[Height] [int] NOT NULL,
	[Text] [varchar](200) NOT NULL,
	[DBField] [varchar](50) NULL,
	[FontColor] [varchar](6) NULL,
	[FontSize] [int] NULL,
	[FontFamily] [varchar](100) NULL,
	[ImageURL] [varchar](200) NULL,
	[ImageColor] [varchar](6) NULL,
	[Description] [varchar](200) NULL,
	[IsDefault] [bit] NOT NULL,
	[CardSide] [bit] NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_tblIdTelField] PRIMARY KEY CLUSTERED 
(
	[Field_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 - Front; 0 -  Back' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblIdTelField', @level2type=N'COLUMN',@level2name=N'CardSide'
GO
/****** Object:  Table [dbo].[tblIdTelDBRelatedField]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIdTelDBRelatedField](
	[DBField] [varchar](50) NOT NULL,
	[Application_FK] [int] NOT NULL,
	[IdTelDBRelatedField_PK] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblIdTelDBRelatedField] PRIMARY KEY CLUSTERED 
(
	[IdTelDBRelatedField_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIdTelBusCardSuspension]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIdTelBusCardSuspension](
	[Suspension_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateStart] [datetime] NOT NULL,
	[DateFinish] [datetime] NOT NULL,
	[Remarks] [varchar](1000) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblIdTelBusCardSuspension] PRIMARY KEY CLUSTERED 
(
	[Suspension_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIdTelBusCardRenewal]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIdTelBusCardRenewal](
	[Renewal_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](25) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateRenewed] [datetime] NOT NULL,
	[Remarks] [varchar](500) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblIdTelBusCardRenewal] PRIMARY KEY CLUSTERED 
(
	[Renewal_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIdTelBusCardHistory]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIdTelBusCardHistory](
	[History_PK] [int] IDENTITY(1,1) NOT NULL,
	[DateEdited] [datetime] NOT NULL,
	[EditedBy] [varchar](25) NOT NULL,
	[IdTelNo] [varchar](25) NOT NULL,
	[Activity] [varchar](200) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblIdTelBusCardHistory] PRIMARY KEY CLUSTERED 
(
	[History_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIdTelBusCard]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIdTelBusCard](
	[BusCard_PK] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[Createdby] [varchar](25) NULL,
	[IdTelNo] [varchar](25) NOT NULL,
	[FullName] [varchar](100) NULL,
	[Cto_FK] [int] NULL,
	[OtherInfo] [varchar](500) NULL,
	[DateIssue] [datetime] NULL,
	[DateExpiry] [datetime] NULL,
	[DateCheckedOut] [datetime] NULL,
	[Template_FK] [int] NOT NULL,
	[BackTemplate_FK] [int] NOT NULL,
 CONSTRAINT [PK_tblIdTelBusCard] PRIMARY KEY CLUSTERED 
(
	[BusCard_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblIdTelApplication]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblIdTelApplication](
	[Application_PK] [int] NOT NULL,
	[ApplicationName] [varchar](50) NOT NULL,
	[RelatedTable] [varchar](50) NULL,
	[IsMixedMode] [bit] NOT NULL,
 CONSTRAINT [PK_tblIdTelApplication] PRIMARY KEY CLUSTERED 
(
	[Application_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblGenaricNo_new]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblGenaricNo_new](
	[gid] [bigint] IDENTITY(1,1) NOT NULL,
	[HID] [nvarchar](255) NULL,
	[GRegion] [nvarchar](50) NULL,
	[GLocation] [nvarchar](255) NULL,
	[GDutyLocation] [nvarchar](255) NULL,
	[GUnit] [nvarchar](255) NULL,
	[ExtNo] [nvarchar](255) NULL,
	[ExtTitle] [nvarchar](255) NULL,
	[f3] [nvarchar](770) NULL,
	[Gmobile] [varchar](200) NULL,
	[GRdoCall] [nvarchar](255) NULL,
 CONSTRAINT [PK_tblGenaricNo_new] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/*
CREATE FULLTEXT INDEX ON [dbo].[tblGenaricNo_new](
[ExtNo] LANGUAGE [English], 
[ExtTitle] LANGUAGE [English], 
[f3] LANGUAGE [English], 
[GLocation] LANGUAGE [English], 
[Gmobile] LANGUAGE [English], 
[GRdoCall] LANGUAGE [English], 
[GUnit] LANGUAGE [English])
KEY INDEX [PK_tblGenaricNo_new]ON ([FTGenaricNew], FILEGROUP [ftfg_FTGenaricNew])
WITH (CHANGE_TRACKING = OFF, STOPLIST = SYSTEM)
GO
*/
/****** Object:  Table [dbo].[tblGalileoAssets]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblGalileoAssets](
	[GenericDescription] [varchar](1000) NULL,
	[ItemDescription] [varchar](1000) NULL,
	[SerilaNumber] [varchar](100) NULL,
	[Barcode] [varchar](100) NULL,
	[SAULocation1] [varchar](500) NULL,
	[Location] [varchar](500) NULL,
	[IDTelNo] [varchar](50) NULL,
	[StaffName] [varchar](500) NULL,
	[IssueDate] [varchar](50) NULL,
	[OrgSection] [varchar](500) NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblGalileoAssets] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermitWarning]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermitWarning](
	[Warning_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateWarned] [datetime] NULL,
	[Remarks] [varchar](1000) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDriverPermitWarning] PRIMARY KEY CLUSTERED 
(
	[Warning_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermitTest]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermitTest](
	[Test_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateTested] [datetime] NOT NULL,
	[Remarks] [varchar](500) NULL,
	[IsPassedSuccess] [bit] NOT NULL,
	[TestedBy] [varchar](20) NOT NULL,
	[Vehicle_FK] [int] NULL,
	[City_FK] [int] NULL,
	[VehicleType] [varchar](200) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDriverPermitTest] PRIMARY KEY CLUSTERED 
(
	[Test_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermitSuspension]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermitSuspension](
	[Suspension_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateStart] [datetime] NOT NULL,
	[DateFinish] [datetime] NOT NULL,
	[Remarks] [varchar](1000) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDriverPermitSuspension] PRIMARY KEY CLUSTERED 
(
	[Suspension_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermitRenewal]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermitRenewal](
	[Renewal_Pk] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](25) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateRenewed] [datetime] NOT NULL,
	[Remarks] [varchar](500) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDriverPermitRenewal] PRIMARY KEY CLUSTERED 
(
	[Renewal_Pk] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermitHistory]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermitHistory](
	[History_PK] [int] IDENTITY(1,1) NOT NULL,
	[DateEdited] [datetime] NOT NULL,
	[EditedBy] [varchar](25) NOT NULL,
	[IdTelNo] [varchar](25) NOT NULL,
	[Activity] [varchar](200) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDriverPermitHistory] PRIMARY KEY CLUSTERED 
(
	[History_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermitEmails]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermitEmails](
	[emails] [varchar](2000) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermitDocument]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermitDocument](
	[Document_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[URL] [varchar](250) NOT NULL,
	[Title] [varchar](250) NOT NULL,
	[DateOfActivity] [datetime] NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDriverPermitDocument] PRIMARY KEY CLUSTERED 
(
	[Document_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermitAccidentType]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermitAccidentType](
	[AccidentType_PK] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDriverPermitAccidentType] PRIMARY KEY CLUSTERED 
(
	[AccidentType_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermitAccident]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermitAccident](
	[Accident_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateAccident] [datetime] NOT NULL,
	[VehicleNumber] [varchar](20) NOT NULL,
	[AccidentCause] [varchar](200) NULL,
	[AccidentType_FK] [int] NOT NULL,
	[City_FK] [int] NOT NULL,
	[Remarks] [varchar](500) NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDriverPermitAccident] PRIMARY KEY CLUSTERED 
(
	[Accident_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDriverPermit]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDriverPermit](
	[DriverPermit_PK] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[CreatedBy] [varchar](25) NULL,
	[IdTelNo] [varchar](25) NOT NULL,
	[FullName] [varchar](100) NULL,
	[Nationality] [varchar](100) NULL,
	[DateOfBirth] [datetime] NULL,
	[OrgSection] [varchar](255) NULL,
	[HID] [int] NOT NULL,
	[CarlogNumber] [varchar](12) NULL,
	[Rank_FK] [int] NOT NULL,
	[EyeTest] [varchar](200) NULL,
	[Cto_FK] [int] NULL,
	[Vehicle_FK] [int] NULL,
	[ExternalLicenseCountry] [varchar](100) NULL,
	[DateLicenseExp] [datetime] NULL,
	[LicenseCategory] [varchar](25) NULL,
	[OtherInfo] [varchar](500) NULL,
	[DateIssue] [datetime] NULL,
	[DateExpiry] [datetime] NULL,
	[DateCheckedOut] [datetime] NULL,
	[CheckedOutRemarks] [varchar](500) NULL,
	[Template_FK] [int] NOT NULL,
	[BackTemplate_FK] [int] NOT NULL,
	[VehicleType] [varchar](200) NULL,
 CONSTRAINT [PK_tblDriverPermit] PRIMARY KEY CLUSTERED 
(
	[DriverPermit_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDocument]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDocument](
	[Document_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](50) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[URL] [varchar](250) NOT NULL,
	[Title] [varchar](250) NOT NULL,
	[DateOfActivity] [datetime] NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDocument] PRIMARY KEY CLUSTERED 
(
	[Document_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDependents]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDependents](
	[IDNo] [varchar](20) NOT NULL,
	[DepFirstName] [varchar](50) NULL,
	[DepLastName] [varchar](50) NULL,
	[DepRelationship] [varchar](30) NULL,
	[DepGender] [varchar](10) NULL,
	[DepDateOfBirth] [varchar](50) NULL,
	[IndexNo] [varchar](20) NOT NULL,
	[Dependants_PK] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblDependents] PRIMARY KEY CLUSTERED 
(
	[Dependants_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCto]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCto](
	[Cto_PK] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblCto] PRIMARY KEY CLUSTERED 
(
	[Cto_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCity]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCity](
	[City_PK] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_tblCity] PRIMARY KEY CLUSTERED 
(
	[City_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBloodGroup]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBloodGroup](
	[BloodGroup_PK] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](50) NOT NULL,
	[IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_tblBloodGroup] PRIMARY KEY CLUSTERED 
(
	[BloodGroup_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblApplication]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblApplication](
	[Application_PK] [int] NOT NULL,
	[ApplicationName] [varchar](200) NOT NULL,
 CONSTRAINT [PK_tblApplication] PRIMARY KEY CLUSTERED 
(
	[Application_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAdmin]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAdmin](
	[AdminID] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[RoleID] [int] NULL,
 CONSTRAINT [PK_tblAdmin] PRIMARY KEY CLUSTERED 
(
	[AdminID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAccessibleFocalPoint]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAccessibleFocalPoint](
	[FocalPoint_PK] [int] IDENTITY(1,1) NOT NULL,
	[IdTelNo] [varchar](20) NOT NULL,
	[AccessibleValue] [varchar](800) NOT NULL,
 CONSTRAINT [PK_tblAccessibleFocalPoint] PRIMARY KEY CLUSTERED 
(
	[FocalPoint_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_ValidIDS]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_ValidIDS](
	[ValidIDS] [varchar](50) NULL,
	[id] [int] NOT NULL,
	[ValidIDS_PK] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tbl_ValidIDS] PRIMARY KEY CLUSTERED 
(
	[ValidIDS_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[staff-intl-natl-fpms]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[staff-intl-natl-fpms](
	[F1] [nvarchar](255) NULL,
	[F2] [nvarchar](255) NULL,
	[ADJAHOUINOU, Dominique] [nvarchar](255) NULL,
	[F4] [float] NULL,
	[Consultant] [nvarchar](255) NULL,
	[AG-0048] [nvarchar](255) NULL,
	[F7] [nvarchar](255) NULL,
	[Staff_PK] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_staff-intl-natl-fpms] PRIMARY KEY CLUSTERED 
(
	[Staff_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMaster_SearchTelDirGeneric]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_PersonnelMaster_SearchTelDirGeneric]
@kword varchar(800)
AS
declare @sqlVar varchar(1500)
select @sqlVar = 'select * from tblgenaricno where '
+@kword
+' order by glocation'
execute(@sqlvar)
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMaster_SearchTelDir]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_PersonnelMaster_SearchTelDir]
@kword varchar(800)
AS
declare @sqlVar varchar(1500)
select @sqlVar = 'SELECT  tblPersonnelMaster.IDTelNo, '
+ 'tblPersonnelMaster.PersonnelUniqueID, '
+ 'tblPersonnelMaster.FirstName + '' '' + tblPersonnelMaster.LastName as fullname, '
+ 'tblPersonnelMaster.Location, '
+ 'tblPersonnelMaster.missionfunctionaltitle, '
+ 'tblPersonnelMaster.LotusNotesInternetAddress, '
+ 'tblPersonnelMaster.MissionPABXExtention, '
+ 'tblPersonnelMaster.pabxprefix, '
+ 'tblPersonnelMaster.isregistered, '
+ 'tblPersonnelMaster.MissionOfficialCellularNo, '
+ 'tblsections.Orgsection AS Sorgsec,  '
+ 'tblsections.Unit AS SUnit,  '
+ 'tblsections.Subunit AS Ssunit, '
+ 'tblsections.Organization AS Sorg  '
+ 'FROM  tblPersonnelMaster  '
+ 'LEFT OUTER JOIN tblsections ON tblPersonnelMaster.HID = tblsections.id  '
+ 'LEFT OUTER JOIN tblIDTelTextDataImport ON tblPersonnelMaster.IDTelNo = tblIDTelTextDataImport.IDTelNo  '
+ 'WHERE ' + @kword + ' '
+ 'and tblsections.Organization = ''MONUC'' '
+ 'and (tblPersonnelMaster.DepartureDate IS NULL  '
+ 'OR tblPersonnelMaster.DepartureDate = 01/01/1900  '
+ 'OR tblIDTelTextDataImport.idtelExpiryDate > { fn NOW() })   '
+ 'and tblPersonnelMaster.HideTelDir <> ''Y'' '
execute(@sqlvar)
GO
/****** Object:  StoredProcedure [dbo].[sp_AdminRole_Delete]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_AdminRole_Delete]
@AdminID int
AS
DELETE FROM tblAdminRole WHERE AdminID = @AdminID
GO
/****** Object:  Table [dbo].[Results]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Results](
	[LookupKey] [varchar](35) NOT NULL,
	[LookupValue] [varchar](40) NOT NULL,
	[LookupValueInternal] [varchar](40) NULL,
	[Results_PK] [int] IDENTITY(1,1) NOT NULL,
	[msrepl_tran_version] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Results] PRIMARY KEY CLUSTERED 
(
	[Results_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FSSTemp]    Script Date: 01/28/2013 17:38:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FSSTemp](
	[Staff_ID] [nvarchar](255) NULL,
	[Username] [nvarchar](255) NULL,
	[ID_Card_Number] [nvarchar](255) NULL,
	[FirstName] [nvarchar](255) NULL,
	[MidName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[DOB] [datetime] NULL,
	[Nationality] [nvarchar](255) NULL,
	[Mail_Address] [nvarchar](255) NULL,
	[Region] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_ISPermissions_LoadDefault]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_ISPermissions_LoadDefault]
AS
SELECT Title, URL
FROM tblISPermissions
WHERE IsRoleBased = 0
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMasterISRoles_GetAll]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- sp_PersonnelMasterISRoles_GetAll '', 0

CREATE        PROCEDURE [dbo].[sp_PersonnelMasterISRoles_GetAll]
@IdTelNo	varchar(20),
@ISRole_FK	int
AS
DECLARE @vSQL varchar(1000)
SELECT @vSQL = 	'SELECT tpr.IdTelNo, ISRole_FK, PersonnelMasterISRole_PK, tr.Title, URL, tpm.FirstName + '' '' + tpm.LastName AS FullName '
		+ 'FROM tblPersonnelMasterISRoles tpr, tblISRoles tr, tblPersonnelMaster tpm '
		+ 'WHERE tpr.IdTelNo like ''%' + @IdTelNo + '%'' '
		+ 'AND tpr.ISRole_FK = tr.ISRole_PK '
		+ 'AND tpm.IDTelNo = tpr.IDTelNo '
IF @ISRole_FK > 0 BEGIN
	SELECT @vSQL = @vSQL + ' AND ISRole_FK = ' + cast(@ISRole_FK AS varchar)
END
SELECT @vSQL = @vSQL + ' ORDER BY tr.Title, tpr.IdTelNO'
print @vSQL
Execute (@vSQL)
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMaster_SearchTelDirSelectedVal]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_PersonnelMaster_SearchTelDirSelectedVal] 
@kword varchar(800)
AS
declare @sqlVar varchar(1500)
select @sqlVar = 'SELECT  tblPersonnelMaster.IDTelNo, '
+ 'tblPersonnelMaster.PersonnelUniqueID, '
+ 'tblPersonnelMaster.FirstName + '' '' + tblPersonnelMaster.LastName as fullname, '
+ 'tblPersonnelMaster.Location, '
+ 'tblPersonnelMaster.missionfunctionaltitle, '
+ 'tblPersonnelMaster.LotusNotesInternetAddress, '
+ 'tblPersonnelMaster.MissionPABXExtention, '
+ 'tblPersonnelMaster.pabxprefix, '
+ 'tblPersonnelMaster.isregistered, '
+ 'tblPersonnelMaster.MissionOfficialCellularNo, '
+ 'tblsections.Orgsection AS Sorgsec,  '
+ 'tblsections.Unit AS SUnit,  '
+ 'tblsections.Subunit AS Ssunit, '
+ 'tblsections.Organization AS Sorg  '
+ 'FROM  tblPersonnelMaster  '
+ 'LEFT OUTER JOIN tblsections ON tblPersonnelMaster.HID = tblsections.id  '
+ 'LEFT OUTER JOIN tblIDTelTextDataImport ON tblPersonnelMaster.IDTelNo = tblIDTelTextDataImport.IDTelNo  '
+ 'WHERE ' 
+ 'tblsections.Organization = ''MONUC'' '
+ 'and (tblPersonnelMaster.DepartureDate IS NULL  '
+ 'OR tblPersonnelMaster.DepartureDate = 01/01/1900  '
+ 'OR tblIDTelTextDataImport.idtelExpiryDate > { fn NOW() })   '
+ 'and tblPersonnelMaster.HideTelDir <> ''Y'' '
+ @kword 
execute(@sqlvar)
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMaster_SearchTelDirKword]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_PersonnelMaster_SearchTelDirKword]
@kword varchar(800)
AS
SET @kword = N'"' + @kword + '*" OR "' + @kword + '*"'
PRINT @kword
SELECT  tpm.IDTelNo,
tpm.PersonnelUniqueID,
tpm.FirstName +' '+ tpm.LastName as fullname, 
tpm.Location, 
tpm.missionfunctionaltitle, 
tpm.LotusNotesInternetAddress, 
tpm.MissionPABXExtention, 
tpm.pabxprefix, 
tpm.isregistered, 
tpm.MissionOfficialCellularNo, 
ts.Orgsection AS Sorgsec,  
ts.Unit AS SUnit,  
ts.Subunit AS Ssunit, 
ts.Organization AS Sorg  
FROM  tblPersonnelMaster tpm
LEFT OUTER JOIN [tblSections ] ts ON tpm.HID = ts.id  
LEFT OUTER JOIN tblIDTelTextDataImport ON tpm.IDTelNo = tblIDTelTextDataImport.IDTelNo  
INNER JOIN CONTAINSTABLE(tblPersonnelMaster, *, @kword) AS KEY_TBL ON KEY_TBL.[KEY] = tpm.PersonnelUniqueID 
WHERE ts.Organization = 'MONUSCO'
and (tpm.DepartureDate IS NULL  
OR tpm.DepartureDate = 01/01/1900  
OR tblIDTelTextDataImport.idtelExpiryDate > { fn NOW() })   
and tpm.HideTelDir <> 'Y'
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMasterISRoles_Get]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE    PROCEDURE [dbo].[sp_PersonnelMasterISRoles_Get]
@ID int
AS
SELECT * FROM tblPersonnelMasterISRoles WHERE PersonnelMasterISRole_PK = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMasterISRoles_Delete]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE    PROCEDURE [dbo].[sp_PersonnelMasterISRoles_Delete]
@ID	int
AS
DELETE FROM tblPersonnelMasterISRoles 
WHERE PersonnelMasterISRole_PK = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMaster_UpdatePassword]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_PersonnelMaster_UpdatePassword]
@IdTelNo	varchar(20),
@Password	varbinary(20)
AS
UPDATE tblPersonnelMaster SET
	[Password] = @Password
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_Application_GetAll]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE    PROCEDURE [dbo].[sp_Application_GetAll]
AS
SELECT *
FROM tblIdTelApplication
ORDER BY ApplicationName
GO
/****** Object:  StoredProcedure [dbo].[sp_Application_Get]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Application_Get]	@Application_PK intASSET NOCOUNT ONSELECT [Application_PK], 	[ApplicationName], 	[RelatedTable], 	[IsMixedMode]FROM tblIdTelApplicationWHERE [Application_PK] = @Application_PK
GO
/****** Object:  StoredProcedure [dbo].[sp_Admin_Insert]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[sp_Admin_Insert]
@Application_FK int,
@Adminname 	VarChar(50),
@RoleID		int,
@ID		int OUTPUT
AS
DECLARE @ISRole int
INSERT INTO tblAdmin (IdTelNo, RoleID)
VALUES (@Adminname, @RoleID)
SET @ID = @@IDENTITY
SELECT @ISRole = ISRole_PK FROM tblISRoles WHERE Application_FK = @Application_FK
INSERT INTO tblPersonnelMasterISRoles (IdTelNo, ISRole_FK)
VALUES (@Adminname, @ISRole)
GO
/****** Object:  StoredProcedure [dbo].[sp_Admin_GetStandalonePermissions]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  sp_Admin_GetStandalonePermissions 4

CREATE PROCEDURE [dbo].[sp_Admin_GetStandalonePermissions]
@Application_FK	int
AS
SELECT DISTINCT PermissionID, PermissionName, PermissionFilename, IsMenuIncluded, OrderViewed
FROM tblPermission
WHERE Application_FK = @Application_FK
ORDER BY OrderViewed, IsMenuIncluded DESC, PermissionID ASC
GO
/****** Object:  StoredProcedure [dbo].[sp_Admin_GetPermissions]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  sp_Admin_GetPermissions 'I-1222', 1

CREATE         PROCEDURE [dbo].[sp_Admin_GetPermissions]
@IdTelNo 		varchar(20),
@Application_FK		int
AS
DECLARE @isSuperAdmin int

SELECT @isSuperAdmin = COUNT(AdminID) 
FROM tblAdmin ta
	LEFT JOIN tblRole tr ON tr.RoleID = ta.RoleID
WHERE tr.ApplicationID = 0
	AND ta.IdTelNo = @IdTelNo
print @isSuperAdmin
IF @isSuperAdmin = 0 BEGIN
	SELECT DISTINCT * FROM (
		SELECT tp.PermissionID, tp.PermissionName, tp.PermissionFilename, tp.IsMenuIncluded, tp.IsSecured, tp.OrderViewed, tp.Application_FK
		FROM tblAdmin ta, tblPermission tp, tblRolePermission trp, tblRole tr
		WHERE ta.IdTelNo = @IdTelNo
			AND ta.RoleID = tr.RoleID
			AND trp.RoleID = tr.RoleID
			AND tp.PermissionID = trp.PermissionID
			AND tr.ApplicationID = @Application_FK
		UNION
		SELECT tp.PermissionID, tp.PermissionName, tp.PermissionFilename, tp.IsMenuIncluded, tp.IsSecured, tp.OrderViewed, tp.Application_FK
		FROM tblAdmin ta, tblPermission tp, tblIdTelApplication tia
		WHERE tia.Application_PK = tp.Application_FK
			AND tp.Application_FK = @Application_FK 
			AND IsSecured = 0 
			AND tia.IsMixedMode = 1

	) AS x
	ORDER BY OrderViewed, IsMenuIncluded DESC, PermissionID ASC
END ELSE BEGIN
	SELECT PermissionID, PermissionName, PermissionFilename, IsMenuIncluded, IsSecured, OrderViewed, Application_FK
	FROM tblPermission
	WHERE Application_FK = @Application_FK 
		OR Application_FK = 0
	ORDER BY OrderViewed, IsMenuIncluded DESC, PermissionID ASC
END

SELECT ApplicationName FROM tblIdTelApplication WHERE Application_PK = @Application_FK
GO
/****** Object:  StoredProcedure [dbo].[sp_Admin_GetAll]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  sp_Admin_GetAll 19,-1,''
--  SELECT * FROM tblAdmin

CREATE    PROCEDURE [dbo].[sp_Admin_GetAll]
@Application_FK	int,
@RoleID 	int,
@IdTelNo 	varchar(20)
AS
IF @RoleID > -1 BEGIN
	SELECT ta.AdminID, ta.IdTelNo, IsNull(ta.RoleID, -1) AS RoleID, LTRIM(tpm.FirstName) + ' ' + tpm.LastName AS FullName
	FROM tblAdmin ta
		LEFT JOIN tblRole tr ON tr.RoleID = ta.RoleID
		LEFT JOIN tblPersonnelMaster tpm ON tpm.IDTelNo = ta.IDTelNo
	WHERE ta.IdTelNo like '%' + @IdTelNo + '%' 
		AND ta.RoleID = @RoleID
		AND tr.ApplicationID = @Application_FK
	ORDER BY FullName ASC
	
END ELSE BEGIN
	SELECT ta.AdminID, ta.IdTelNo, IsNull(ta.RoleID, -1) AS RoleID, LTRIM(tpm.FirstName) + ' ' + tpm.LastName AS FullName
	FROM tblAdmin ta
		LEFT JOIN tblRole tr ON tr.RoleID = ta.RoleID
		LEFT JOIN tblPersonnelMaster tpm ON tpm.IDTelNo = ta.IDTelNo
	WHERE ta.IdTelNo like '%' + @IdTelNo + '%' 
		AND tr.ApplicationID = @Application_FK
	ORDER BY FullName ASC
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Admin_Delete]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Admin_Delete]
@ID int
AS
DELETE FROM tblAdmin WHERE AdminID = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_Admin_AssignRole]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Admin_AssignRole]
@AdminID	int,
@RoleID		int
AS
UPDATE tblAdmin 
SET
	RoleID = @RoleID
WHERE AdminID = @AdminID
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMaster_Login]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_PersonnelMaster_Login]
@IdTelNo	varchar(20),
@Password	varbinary(20)
AS
SELECT LotusNotesInternetAddress, FirstName, LastName, IDTelNo, Location, IDTelName, OrgSection,
	(
	SELECT IsRegistered
	FROM tblPersonnelMaster 
	WHERE IDTelNo = @IdTelNo and isregistered=1 and ( CAST(tblPersonnelMaster.Password AS varchar) <> '' and CAST(tblPersonnelMaster.Password AS varchar) is not null )
	) AS IsRegistered

FROM tblPersonnelMaster 
WHERE [Password] = @Password 
	AND [Password] <> '' 
	AND IDTelNo = @IdTelNo

SELECT DISTINCT tp.Title, tp.URL 
FROM tblPersonnelMasterISRoles tpr, tblISRoles tr, tblISPermissions tp, tblISRolePermissions trp
WHERE tpr.IDTelNo = @IdTelNo
	AND tpr.ISRole_FK = tr.ISRole_PK
	AND tr.ISRole_PK = trp.ISRole_FK
	AND trp.ISPermission_FK = tp.ISPermission_PK
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMaster_GetMembersEmail]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_PersonnelMaster_GetMembersEmail]
	@PermissionFilename varchar(100)
AS
SELECT tpm.LotusNotesInternetAddress AS Email
FROM tblPersonnelMaster tpm
	LEFT JOIN tblAdmin ta ON tpm.IDTelNo = ta.IdTelNo
	LEFT JOIN tblRolePermission trp on trp.RoleID = ta.RoleID
	LEFT JOIN tblPermission tp ON tp.PermissionID = trp.PermissionID
WHERE tp.PermissionFilename like '%' + @PermissionFilename + '%'
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_ValidateIdTelNo]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  sp_PersonnelInfo_ValidateIdTelNo 'I-1220'

CREATE       PROCEDURE [dbo].[sp_PersonnelInfo_ValidateIdTelNo]
@IdTelNo	varchar(20)
AS
--SELECT 1 FROM tblPersonnelMaster
--WHERE IdTelNO = @IdTelNo
UPDATE tblPersonnelMaster SET
	IdTelNo = @IdTelNo
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_UpdateSignature]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_PersonnelInfo_UpdateSignature]
@IdTelNo	varchar(20)
AS
UPDATE tblPersonnelMaster SET
	SignatureFilename = @IdTelNo + '.jpg'
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_UpdateSecurityDetails]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_PersonnelInfo_UpdateSecurityDetails]
@IdTelNo		varchar(20),
@MissionCountry		varchar(30),
@City			varchar(30),
@Street			varchar(50),
@StreetNumber		varchar(50),
@Building		varchar(50),
@AppartmentNo		varchar(50),
@SecurityZoneNo		varchar(50),
@Email				varchar(100),
@LastUpdatedBy		varchar(20)
AS
UPDATE tblPersonnelMaster SET
	IdTelNo = @IdTelNo,
	MissionCountry = @MissionCountry, 
	Location = @City, 
	StreetNo = @Street, 
	StreetNumber = @StreetNumber, 
	BuildingNo = @Building, 
	AppartmentNo = @AppartmentNo, 
	SecurityZoneNo = @SecurityZoneNo,
	LotusNotesInternetAddress = @Email,
	IsRegistered = 1,
	LastUpdatedOn = GetDate(),
	LastUpdatedBy = @LastUpdatedBy
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_UpdateProfile]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE        PROCEDURE [dbo].[sp_PersonnelInfo_UpdateProfile]
@IdTelNo			varchar(20),
@HID				int,
@FirstName			varchar(50),
@LastName			varchar(50),
@PersonalCellularNo		varchar(20) = null,
@OfficialCellularNo		varchar(20) = null,
@MissionPABXExtention		varchar(10) = null,
@PABXPrefix			varchar(10) = null,
@LotusNotesInternetAddress	varchar(50),
@Region				varchar(50) = null,
@DutyLocation			varchar(50) = null,
@LastUpdatedBy			varchar(20),
@DisplayMyMobile		bit,
@MissionRadioCallsign		varchar(20) = null,
@MissionTrunkingNo		varchar(20) = null
AS
UPDATE tblPersonnelMaster SET
	IdTelNo = @IdTelNo,
	HID = @HID,
	FirstName = @FirstName,
	LastName = @LastName,
	MobilePhoneNos = @PersonalCellularNo,
	MissionOfficialCellularNo = @OfficialCellularNo,
	MissionPABXExtention = @MissionPABXExtention,
	PABXPrefix = @PABXPrefix,
	LotusNotesInternetAddress = @LotusNotesInternetAddress,
	Region = @Region,
	DutyStation = @DutyLocation,
	LastUpdatedOn = GetDate(),
	LastUpdatedBy = @LastUpdatedBy,
	DisplayMyMobile = @DisplayMyMobile,
	MissionRadioCallsign = @MissionRadioCallsign,
	MissionTrunkingNo = @MissionTrunkingNo
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_UpdatePhoto]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_PersonnelInfo_UpdatePhoto]
@IdTelNo	varchar(20)
AS
UPDATE tblPersonnelMaster SET
	PhotoFilename = @IdTelNo + '.jpg'
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_UpdatePassword]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE  PROCEDURE [dbo].[sp_PersonnelInfo_UpdatePassword]
@IdTelNo	varchar(20),
@Password	varbinary(20)
AS
UPDATE tblPersonnelMaster SET
	[Password] = @Password
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_UpdateLNEmail]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_PersonnelInfo_UpdateLNEmail]
@IdTelNo			varchar(20),
@LotusNotesInternetAddress	varchar(50)
AS
UPDATE tblPersonnelMaster SET
	LotusNotesInternetAddress = @LotusNotesInternetAddress
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_UpdateContactDetails]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE    PROCEDURE [dbo].[sp_PersonnelInfo_UpdateContactDetails]
@IdTelNo			varchar(20),
@UnIndexNo			varchar(10),
@CountryOriginal	varchar(50),
@HID				int,
@OtherOrganization	varchar(50),
@DateOfBirth		DateTime,
@Weight				varchar(10),
@HairColor			varchar(50),
@MobilePhoneNos		varchar(20),
@LastUpdatedBy		varchar(20)
AS
UPDATE tblPersonnelMaster SET
	IdTelNo = @IdTelNo,
	UnIndexNo = @UnIndexNo, 
	CountryOriginal = @CountryOriginal, 
	HID = @HID, 
	Organization = @OtherOrganization,
	DateOfBirth = @DateOfBirth, 
	Weight = @Weight, 
	HairColor = @HairColor, 
	MobilePhoneNos = @MobilePhoneNos,
	LastUpdatedBy = @LastUpdatedBy,
	LastUpdatedOn = GetDate()
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_Update]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE         PROCEDURE [dbo].[sp_PersonnelInfo_Update]
@ID			int,
@IdTelNo		varchar(20),
@FirstName		varchar(50),
@LastName		varchar(50),
@Title			varchar(100),
@ExpiryDate		datetime,
@BloodType		varchar(8),
@Height			varchar(10),
@EyeColor		varchar(15),
@TemplateID		int,
@BackTemplateID	int,
@DutyLocation	varchar(50),
@LastUpdatedBy	varchar(20)
AS
UPDATE tblPersonnelMaster SET
	IdTelNo = @IdTelNo,
	FirstName = @FirstName, 
	LastName = @LastName, 
	MissionFunctionalTitle = @Title, 
	ExpiryDate = @ExpiryDate, 
	BloodType = @BloodType, 
	Height = @Height, 
	EyeColor = @EyeColor,
	Template_FK = @TemplateID,
	BackTemplate_FK = @BackTemplateID,
	DutyStation = @DutyLocation,
	LastUpdatedBy = @LastUpdatedBy,
	LastUpdatedOn = GetDate()
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_ResetPassword]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[sp_PersonnelInfo_ResetPassword]
@IdTelNo	varchar(20),
@OldPassword	varbinary(20),
@NewPassword	varbinary(20)
AS
UPDATE tblPersonnelMaster SET
	[Password] = @NewPassword
WHERE IdTelNo = @IdTelNo
	AND [Password] = @OldPassword
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_InsertGenericAccount]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE            PROCEDURE [dbo].[sp_PersonnelInfo_InsertGenericAccount]
@FirstName		varchar(50),
@LastName		varchar(50),
@Title			varchar(100),
@Email			varchar(50),
@LastUpdatedBy		varchar(20),
@GenericString		varchar(10),
@ID			varchar(20) OUTPUT
AS

DECLARE @GenericLength int
DECLARE @LastGenericID varchar(20)
DECLARE @NewGenericNumber varchar(20)

DECLARE @Length int
DECLARE @RandomID varchar(32)
DECLARE @counter smallint
DECLARE @RandomNumber float
DECLARE @RandomNumberInt tinyint
DECLARE @CurrentCharacter varchar(1)
DECLARE @ValidCharacters varchar(255)
DECLARE @ValidCharactersLength int
SET @ValidCharacters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-=+&$'
SET @ValidCharactersLength = len(@ValidCharacters)
SET @CurrentCharacter = ''
SET @RandomNumber = 0
SET @RandomNumberInt = 0
SET @RandomID = ''
SET @Length = 8

SET NOCOUNT ON

SET @counter = 1

WHILE @counter < (@Length + 1) BEGIN

        SET @RandomNumber = Rand()
        SET @RandomNumberInt = Convert(tinyint, ((@ValidCharactersLength - 1) * @RandomNumber + 1))

        SELECT @CurrentCharacter = SUBSTRING(@ValidCharacters, @RandomNumberInt, 1)

        SET @counter = @counter + 1

        SET @RandomID = @RandomID + @CurrentCharacter

END

SET @counter = 0
SET @GenericLength = len(@GenericString)

SELECT TOP 1 @LastGenericID = IdTelNo
FROM tblPersonnelMaster 
WHERE IdTelNo like '%' + @GenericString + '%'
ORDER BY IdTelNo DESC

SET @Length = len(@LastGenericID)
SET @NewGenericNumber = SUBSTRING(@LastGenericID, @GenericLength+1, @Length) + 1
WHILE @counter + @GenericLength + len(@NewGenericNumber) < @Length BEGIN
	SET @NewGenericNumber = '0' + @NewGenericNumber
END
SET @ID = @GenericString + @NewGenericNumber
INSERT INTO tblPersonnelMaster (IdTelNo, FirstName, LastName, MissionFunctionalTitle, LotusNotesInternetAddress, ExpiryDate, [Password], IsRegistered, LastUpdatedBy, LastUpdatedOn, CreatedBy, CreatedOn )
VALUES (@ID, @FirstName, @LastName, @Title, @Email, '01/01/2199', CAST(@RandomID AS varbinary), 0, @LastUpdatedBy, GetDate(), @LastUpdatedBy, GetDate() )
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_Insert]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  sp_PersonnelInfo_Insert 'I-4444', 'Pascal', 'Harvey', 'Webmaster', '', 'O+', '154', 'Blond', 0

CREATE           PROCEDURE [dbo].[sp_PersonnelInfo_Insert]
@IdTelNo		varchar(20),
@IDTelName		varchar(50),
@FirstName		varchar(50),
@LastName		varchar(50),
@Title			varchar(100),
@ExpiryDate		datetime,
@BloodType		varchar(50) = null,
@Height			varchar(10) = null,
@EyeColor		varchar(15) = null,
@TemplateID		int = null,
@BackTemplateID	int = null,
@DutyLocation	varchar(50),
@LastUpdatedBy	varchar(20),
@ID				int OUTPUT
AS
DECLARE @NbRow int
SELECT @NbRow = COUNT(IdTelNo)
FROM tblPersonnelMaster
WHERE IdTelNo = @IdTelNo
IF @NbRow > 0 BEGIN
	SET @ID = 0
END ELSE BEGIN

	DECLARE @Length int
	DECLARE @RandomID varchar(32)
	DECLARE @counter smallint
	DECLARE @RandomNumber float
	DECLARE @RandomNumberInt tinyint
	DECLARE @CurrentCharacter varchar(1)
	DECLARE @ValidCharacters varchar(255)
	DECLARE @ValidCharactersLength int
	SET @ValidCharacters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-=+&$'
	SET @ValidCharactersLength = len(@ValidCharacters)
	SET @CurrentCharacter = ''
	SET @RandomNumber = 0
	SET @RandomNumberInt = 0
	SET @RandomID = ''
	SET @Length = 8
	
	SET NOCOUNT ON
	
	SET @counter = 1
	
	WHILE @counter < (@Length + 1) BEGIN
	
	        SET @RandomNumber = Rand()
	        SET @RandomNumberInt = Convert(tinyint, ((@ValidCharactersLength - 1) * @RandomNumber + 1))
	
	        SELECT @CurrentCharacter = SUBSTRING(@ValidCharacters, @RandomNumberInt, 1)
	
	        SET @counter = @counter + 1
	
	        SET @RandomID = @RandomID + @CurrentCharacter
	
	END
	INSERT INTO tblPersonnelMaster (IdTelNo, IDTelName, FirstName, LastName, MissionFunctionalTitle, ExpiryDate, BloodType, Height, EyeColor, Template_FK, BackTemplate_FK, DutyStation, [Password], IsRegistered, LastUpdatedBy, LastUpdatedOn, CreatedBy, CreatedOn )
	VALUES (@IdTelNo, @IDTelName, @FirstName, @LastName, @Title, @ExpiryDate, @BloodType, @Height, @EyeColor, @TemplateID, @BackTemplateID, @DutyLocation, CAST(@RandomID AS varbinary), 0, @LastUpdatedBy, GetDate(), @LastUpdatedBy, GetDate() )
	SET @ID = @@IDENTITY
	
END
PRINT @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_GetSearch]    Script Date: 01/28/2013 17:38:41 ******/
/*
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE         PROCEDURE [dbo].[sp_PersonnelInfo_GetSearch]
@Info	varchar(250)
AS
SET @Info = @Info + '* OR "' + @Info + '*"'
DECLARE @tblP TABLE(id bigint primary key)
INSERT INTO @tblP SELECT [KEY] FROM CONTAINSTABLE(tblPersonnelMaster,*, @Info)
DECLARE @tblS TABLE(id int primary key)
INSERT INTO @tblS SELECT [KEY] FROM CONTAINSTABLE(tblSections,OrgSection, @Info)

SELECT IDTelNo, FirstName, LastName, S.OrgSection, MissionPABXExtention, LotusNotesInternetAddress,
	(SELECT COUNT(IdTelNo) FROM tblLeave WHERE IdTelNO = tpm.IdTelNo AND DateFrom <= GetDate()) AS Status
FROM 	tblPersonnelMaster tpm, tblSections S
WHERE tpm.HID = S.[ID] AND (DepartureDate IS NULL OR DepartureDate < GetDate()-5000 OR DepartureDate > GetDate())
	AND (Exists(SELECT id from @tblP where id = tpm.PersonnelUniqueID) OR Exists(SELECT id from @tblS where id = S.id))
GO
*/
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_GetLogin]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE          PROCEDURE [dbo].[sp_PersonnelInfo_GetLogin]
@IdTelNo	varchar(20),
@Password	varbinary(20)
AS
SELECT tpm.LotusNotesInternetAddress, tpm.FirstName, tpm.LastName, tpm.IDTelNo, tpm.Location, tpm.IDTelName, ts.OrgSection, tpm.ExpiryDate, tpm.DepartureDate,
	(
	SELECT IsRegistered
	FROM tblPersonnelMaster 
	WHERE IDTelNo = @IdTelNo 
		and ( CAST(tblPersonnelMaster.Password AS varchar) <> '' 
		and CAST(tblPersonnelMaster.Password AS varchar) is not null )
	) AS IsRegistered,
	(
	SELECT 1
	FROM tblPersonnelMaster
	WHERE IDTelNo = @IdTelNO 
		and ExpiryDate >= GetDate() 
		and ( IsNull(DepartureDate, '01/01/1900') = '01/01/1900' or DepartureDate >= GetDate() ) 
	) AS IsValid,
	(
	SELECT COUNT(IdTelNo)
	FROM tblLeave
	WHERE IdTelNo = @IdTelNo
		AND DateFrom <= GetDate()
		AND (DateTo >= GetDate()-1 OR DateTo < GetDate()-1000)
	) AS IsAway
FROM tblPersonnelMaster tpm
	LEFT JOIN tblSections ts ON ts.[id] = tpm.HID
WHERE [Password] = @Password 
	AND [Password] <> '' 
	AND IDTelNo = @IdTelNo

SELECT DISTINCT ISRole_PK, Title, URL, IsRoleBased, IsPopUp, IsNull(Application_FK, 0) AS Application_FK
FROM tblPersonnelMasterISRoles tpr, tblISRoles tr
WHERE tpr.IDTelNo = @IdTelNo
	AND tpr.ISRole_FK = tr.ISRole_PK
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_GetIdTelNo]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  sp_PersonnelInfo_GetIdTelNo 'I-1220'

CREATE             PROCEDURE [dbo].[sp_PersonnelInfo_GetIdTelNo] 
@IdTelNo varchar(20)
AS
SELECT tdp.PersonnelUniqueID, IsNull(tdp.CreatedOn, '01/01/1970') AS CreatedOn, tdp.CreatedBy, tdp.IdTelNo, tdp.IdTelName,
	tdp.FirstName, tdp.LastName, tdp.MissionFunctionalTitle, 
	IsNull(tdp.ExpiryDate, '01/01/1970') AS ExpiryDate, tdp.BloodType, 
	tdp.Height, tdp.EyeColor, IsNull(tdp.DateOfBirth, '01/01/1900') AS DateOfBirth, 
	tdp.MissionFunctionalTitle, tdp.Nationality, tdp.SignatureFilename, 
	tdp.PhotoFilename, tdp.UnIndexNo, tdp.CountryOriginal,tdp.HID,
	tdp.Weight,tdp.HairColor, tdp.MobilePhoneNos, tdp.LotusNotesInternetAddress, 
	tdp.MissionPABXExtention, tdp.PABXPrefix, tdp.MissionOfficialCellularNo, tdp.MissionTrunkingNo, tdp.MissionRadioCallsign,
	tdp.MissionCountry, tdp.Location, tdp.StreetNo AS Street, tdp.StreetNumber, tdp.DutyStation, tdp.Region,
	tdp.BuildingNo as Building, tdp.AppartmentNo, tdp.SecurityZoneNo, tdp.Template_FK, tdp.BackTemplate_FK, tdp.Organization AS OtherOrganization,
	ts.Organization, ts.OrgSection, ts.Unit, ts.SubUnit, IsNull(tdp.LastUpdatedOn, '01/01/9999') AS LastUpdatedOn, 
	tdp.LastUpdatedBy, tdp.DisplayMyMobile, DepartureDate, HideTelDir, ExpiryDate
FROM tblPersonnelMaster tdp
	LEFT JOIN tblSections ts ON ts.[id] = tdp.HID
WHERE tdp.IdTelNo = @IdTelNo
SELECT *
FROM tblDocument
WHERE IdTelNo = @IdTelNo
ORDER BY DateCreated DESC
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_GetEmail]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  sp_PersonnelInfo_GetEmail 'I-1220'

CREATE PROCEDURE [dbo].[sp_PersonnelInfo_GetEmail]
@IdTelNo varchar(20)
AS
SELECT LotusNotesInternetAddress
FROM tblPersonnelMaster
WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_GetAllPassword]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_PersonnelInfo_GetAllPassword]
AS
SELECT     IDTelNo, CONVERT(varchar, Password) AS Password
FROM         tblPersonnelMaster
WHERE     (ISNULL(Password, - 1) <> - 1)
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_GetAllIDTelNo]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_PersonnelInfo_GetAllIDTelNo]
@Info	varchar(50)
AS
SELECT tpm.IdTelNo
FROM tblPersonnelMaster tpm
	INNER JOIN CONTAINSTABLE(tblPersonnelMaster, (FirstName, LastName, IDTelNo), @Info) AS KEY_TBL ON KEY_TBL.[KEY] = tpm.PersonnelUniqueID
ORDER BY tpm.LastName DESC
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_GetAll]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_PersonnelInfo_GetAll]
@Info	varchar(50)
AS
SELECT tpm.IdTelNo, tpm.FirstName, tpm.LastName, ts.Organization, ts.OrgSection, tpm.MissionFunctionalTitle, tpm.LotusNotesInternetAddress AS Email, 
	IsNull(tpm.Template_FK, 0) AS Template_FK, tpm.PhotoFilename
FROM tblPersonnelMaster tpm
	LEFT JOIN tblSections ts ON ts.[id] = tpm.HID
	INNER JOIN CONTAINSTABLE(tblPersonnelMaster, (FirstName, LastName, IDTelNo), @Info) AS KEY_TBL ON KEY_TBL.[KEY] = tpm.PersonnelUniqueID
ORDER BY tpm.LastName DESC
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelInfo_Delete]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE    PROCEDURE [dbo].[sp_PersonnelInfo_Delete]
@IdTelNo varchar(20)
AS
DELETE FROM tblPersonnelMaster WHERE IdTelNo = @IdTelNo
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_Update]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_Permission_Update]
@ID			int,
@PermissionName		VarChar(50),
@PermissionFilename 	Varchar(100),
@IsMenuIncluded		bit,
@IsSecured		bit,
@OrderViewed		int,
@Application_FK		int
AS
UPDATE tblPermission
SET
	PermissionName = @PermissionName,
	PermissionFilename = @PermissionFilename,
	IsMenuIncluded = @IsMenuIncluded,
	IsSecured = @IsSecured,
	OrderViewed = @OrderViewed,
	Application_FK = @Application_FK
WHERE PermissionID = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_Insert]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_Permission_Insert]
@PermissionName		VarChar(50),
@PermissionFilename 	Varchar(100),
@IsMenuIncluded		bit,
@IsSecured		bit,
@OrderViewed		int,
@Application_FK		int,
@ID			int OUTPUT
AS
INSERT INTO tblPermission (PermissionName, PermissionFilename, IsMenuIncluded, IsSecured, OrderViewed, Application_FK)
VALUES (@PermissionName, @PermissionFilename, @IsMenuIncluded, @IsSecured, @OrderViewed, @Application_FK)
SET @ID = @@IDENTITY
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_GetAllAssignToRole]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  sp_Permission_GetAllAssignToRole 3, 7
--  SELECT * FROM tblPermission

CREATE    PROCEDURE [dbo].[sp_Permission_GetAllAssignToRole]
@Application_FK	int,
@RoleID		int
AS
SELECT tp.PermissionID, tp.PermissionName, tp.PermissionFilename, tp.OrderViewed, tp.IsMenuIncluded, ta.ApplicationName,
	( SELECT COUNT(*) 
	FROM tblRolePermission trp 
	WHERE trp.RoleID = @RoleID AND trp.PermissionID = tp.PermissionID 
	) AS IsIncluded
FROM tblPermission tp 
	LEFT JOIN tblIdTelApplication ta ON ta.Application_PK = tp.Application_FK
WHERE tp.Application_FK = @Application_FK OR tp.Application_FK = 0
ORDER BY Application_FK, OrderViewed
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_GetAll]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  sp_Permission_GetAll 2

CREATE   PROCEDURE [dbo].[sp_Permission_GetAll]
@Application_FK int
AS
SELECT tp.*, ta.ApplicationName 
FROM tblPermission tp 
	LEFT JOIN tblIdTelApplication ta ON ta.Application_PK = tp.Application_FK
WHERE tp.Application_FK = @Application_FK OR tp.Application_FK = 0
ORDER BY Application_FK, OrderViewed
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_Get]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Permission_Get]
@ID int
AS
SELECT * FROM tblPermission WHERE PermissionID = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_Delete]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Permission_Delete]
@ID int
AS
DELETE FROM tblPermission WHERE PermissionID = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_ClearRolePermissions]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Permission_ClearRolePermissions]
@RoleID int
AS
DELETE FROM tblRolePermission WHERE RoleID = @RoleID
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_AssignPermissionToRole]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Permission_AssignPermissionToRole]
@RoleID		int,
@PermissionID	int
AS
INSERT INTO tblRolePermission (RoleID, PermissionID)
VALUES (@RoleID, @PermissionID)
GO
/****** Object:  StoredProcedure [dbo].[sp_ISRoles_Update]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE        PROCEDURE [dbo].[sp_ISRoles_Update]
@ID		int,
@Application_FK int,
@Title		VarChar(50),
@URL		VarChar(200),
@IsRoleBased	bit,
@IsPopUp	Bit
AS
UPDATE tblISRoles
SET
	Application_FK = @Application_FK,
	Title = @Title,
	URL = @URL,
	IsRoleBased = @IsRoleBased,
	IsPopUp = @IsPopUp
WHERE ISRole_PK = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_ISRoles_LoadDefault]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[sp_ISRoles_LoadDefault]
AS
SELECT ISRole_PK, Title, URL, IsRoleBased, IsPopUp, IsNull(Application_FK, 0) AS Application_FK 
FROM tblISRoles
WHERE IsRoleBased = 0
GO
/****** Object:  StoredProcedure [dbo].[sp_ISRoles_Insert]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE      PROCEDURE [dbo].[sp_ISRoles_Insert]
@Application_FK int,
@Title		VarChar(50),
@URL		VarChar(200),
@IsRoleBased	Bit,
@IsPopUp	Bit,
@ID		int OUTPUT
AS
INSERT INTO tblISRoles (Application_FK,Title,URL,IsRoleBased,IsPopUp)
VALUES (@Application_FK,@Title, @URL, @IsRoleBased, @IsPopUp)
SET @ID = @@IDENTITY
GO
/****** Object:  StoredProcedure [dbo].[sp_ISRoles_GetAll]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE      PROCEDURE [dbo].[sp_ISRoles_GetAll]
AS
SELECT tisr.*, ta.ApplicationName
FROM tblISRoles tisr
	LEFT JOIN tblIdTelApplication ta ON ta.Application_PK = tisr.Application_FK
ORDER BY tisr.Title
GO
/****** Object:  StoredProcedure [dbo].[sp_ISRoles_Get]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[sp_ISRoles_Get]
@ID int
AS
SELECT * FROM tblISRoles WHERE ISRole_PK = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_ISRoles_Delete]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[sp_ISRoles_Delete]
@ID int
AS
DELETE FROM tblISRoles WHERE ISRole_PK = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_Userinfo_Get]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Userinfo_Get]
@IDNumber VarChar(20)
AS
SELECT 	IDTelNo, OrgSection, FirstName, LastName, Nationality, MissionFunctionalTitle, BloodType, DateOfBirth
FROM tblPersonnelMaster
WHERE IDTelNo = @IDNumber
GO
/****** Object:  StoredProcedure [dbo].[sp_Role_Update]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Role_Update]
@ID		int,
@RoleName	VarChar(100)
AS
UPDATE tblRole
SET
	RoleName = @RoleName
WHERE RoleID = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_Role_Insert]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_Role_Insert]
@RoleName	VarChar(100),
@ApplicationID	int,
@ID		int OUTPUT
AS
INSERT INTO tblRole (RoleName, ApplicationID)
VALUES (@RoleName, @ApplicationID)
SET @ID = @@IDENTITY
GO
/****** Object:  StoredProcedure [dbo].[sp_Role_GetAll]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_Role_GetAll]
@ApplicationID	int
AS
SELECT * FROM tblRole WHERE ApplicationID = @ApplicationID
GO
/****** Object:  StoredProcedure [dbo].[sp_Role_Get]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Role_Get]
@ID int
AS
SELECT * FROM tblRole WHERE RoleID = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_Role_Delete]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Role_Delete]
@ID int
AS
DELETE FROM tblRole WHERE RoleID = @ID
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMasterISRoles_Update]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE        PROCEDURE [dbo].[sp_PersonnelMasterISRoles_Update]
@PersonnelMasterISRole_PK	int,
@IdTelNo			varchar(20),
@ISRole_FK			int
AS
UPDATE tblPersonnelMasterISRoles
SET
	IdTelNo = @IdTelNo,
	ISRole_FK = @ISRole_FK
WHERE PersonnelMasterISRole_PK = @PersonnelMasterISRole_PK
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMasterISRoles_InsertAll]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
--  sp_PersonnelMasterISRoles_InsertAll 'Communications and Information Technology', 10

CREATE       PROCEDURE [dbo].[sp_PersonnelMasterISRoles_InsertAll]
@OrgSection	nvarchar(255),
@ISRole_FK	int
AS
DELETE FROM tblPersonnelMasterISRoles
WHERE IdTelNo in (
	SELECT tpm.IdTelNO 
	FROM tblPersonnelMaster tpm, tblPersonnelMasterISRoles tpmr, tblSections ts
	WHERE ts.[ID] = tpm.HID
		AND ts.OrgSection = @OrgSection
		AND tpmr.IdTelNo = tpm.IdTelNo
		AND tpmr.ISRole_FK = @ISRole_FK
)
AND ISRole_FK = @ISRole_FK

INSERT INTO tblPersonnelMasterISRoles 
SELECT tpm.IdTelNo, @ISRole_FK, newid()
FROM tblPersonnelMaster tpm, tblSections ts
WHERE ts.[ID] = tpm.HID
	AND ts.OrgSection = @OrgSection
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonnelMasterISRoles_Insert]    Script Date: 01/28/2013 17:38:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE     PROCEDURE [dbo].[sp_PersonnelMasterISRoles_Insert]
@IdTelNo	varchar(20),
@ISRole_FK	int,
@ID		int OUTPUT
AS
INSERT INTO tblPersonnelMasterISRoles (IdTelNo, ISRole_FK)
VALUES (@IdTelNo, @ISRole_FK)
SET @ID = @@IDENTITY
GO
/****** Object:  Default [DF__Results__msrepl___5244F976]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[Results] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__staff-int__msrep__47C76B03]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[staff-intl-natl-fpms] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblCity_IsDefault]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblCity] ADD  CONSTRAINT [DF_tblCity_IsDefault]  DEFAULT (0) FOR [IsDefault]
GO
/****** Object:  Default [DF_tblCto_IsDefault]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblCto] ADD  CONSTRAINT [DF_tblCto_IsDefault]  DEFAULT (0) FOR [IsDefault]
GO
/****** Object:  Default [DF__tblCto__msrepl_t__62BA8D0A]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblCto] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__tblDepend__msrep__77AB884F]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDependents] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblDocument_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDocument] ADD  CONSTRAINT [DF_tblDocument_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__tblDocume__msrep__6D2DF9DC]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDocument] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblDriverPermit_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermit] ADD  CONSTRAINT [DF_tblDriverPermit_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF_tblDriverPermit_HID_1]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermit] ADD  CONSTRAINT [DF_tblDriverPermit_HID_1]  DEFAULT ((0)) FOR [HID]
GO
/****** Object:  Default [DF_tblDriverPermit_Template_FK]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermit] ADD  CONSTRAINT [DF_tblDriverPermit_Template_FK]  DEFAULT ((-1)) FOR [Template_FK]
GO
/****** Object:  Default [DF_tblDriverPermit_BackTemplate_FK]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermit] ADD  CONSTRAINT [DF_tblDriverPermit_BackTemplate_FK]  DEFAULT ((77)) FOR [BackTemplate_FK]
GO
/****** Object:  Default [DF_tblDriverPermitAccident_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitAccident] ADD  CONSTRAINT [DF_tblDriverPermitAccident_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__tblDriver__msrep__5748DA5E]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitAccident] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__tblDriver__msrep__51900108]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitAccidentType] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblDriverPermitDocument_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitDocument] ADD  CONSTRAINT [DF_tblDriverPermitDocument_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__tblDriver__msrep__4BD727B2]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitDocument] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblDriverPermitHistory_DateEdited]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitHistory] ADD  CONSTRAINT [DF_tblDriverPermitHistory_DateEdited]  DEFAULT (getdate()) FOR [DateEdited]
GO
/****** Object:  Default [DF__tblDriver__msrep__461E4E5C]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitHistory] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblDriverPermitRenewal_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitRenewal] ADD  CONSTRAINT [DF_tblDriverPermitRenewal_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__tblDriver__msrep__40657506]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitRenewal] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblDriverPermitSuspension_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitSuspension] ADD  CONSTRAINT [DF_tblDriverPermitSuspension_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__tblDriver__msrep__3AAC9BB0]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitSuspension] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblDriverPermitTest_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitTest] ADD  CONSTRAINT [DF_tblDriverPermitTest_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__tblDriver__msrep__34F3C25A]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitTest] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblDriverPermitWarning_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitWarning] ADD  CONSTRAINT [DF_tblDriverPermitWarning_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__tblDriver__msrep__2F3AE904]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblDriverPermitWarning] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__tblGalile__msrep__0CDBAF5F]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblGalileoAssets] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblGenaricNo_new_GUnit]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblGenaricNo_new] ADD  CONSTRAINT [DF_tblGenaricNo_new_GUnit]  DEFAULT (N'Generic Numbers') FOR [GUnit]
GO
/****** Object:  Default [DF_tblIdTelApplication_IsMixedMode]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelApplication] ADD  CONSTRAINT [DF_tblIdTelApplication_IsMixedMode]  DEFAULT (0) FOR [IsMixedMode]
GO
/****** Object:  Default [DF_tblIdTelBusCard_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelBusCard] ADD  CONSTRAINT [DF_tblIdTelBusCard_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF_tblIdTelBusCard_BackTemplate_FK]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelBusCard] ADD  CONSTRAINT [DF_tblIdTelBusCard_BackTemplate_FK]  DEFAULT ((-1)) FOR [BackTemplate_FK]
GO
/****** Object:  Default [DF_tblIdTelBusCardHistory_DateEdited]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelBusCardHistory] ADD  CONSTRAINT [DF_tblIdTelBusCardHistory_DateEdited]  DEFAULT (getdate()) FOR [DateEdited]
GO
/****** Object:  Default [DF__tblIdTelB__msrep__185783AC]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelBusCardHistory] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblIdTelBusCardRenewal_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelBusCardRenewal] ADD  CONSTRAINT [DF_tblIdTelBusCardRenewal_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__tblIdTelB__msrep__129EAA56]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelBusCardRenewal] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblIdTelBusCardSuspension_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelBusCardSuspension] ADD  CONSTRAINT [DF_tblIdTelBusCardSuspension_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__tblIdTelB__msrep__0CE5D100]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelBusCardSuspension] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblIdTelField_IsDefault]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelField] ADD  CONSTRAINT [DF_tblIdTelField_IsDefault]  DEFAULT ((0)) FOR [IsDefault]
GO
/****** Object:  Default [DF_tblIdTelField_CardSide]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelField] ADD  CONSTRAINT [DF_tblIdTelField_CardSide]  DEFAULT ((1)) FOR [CardSide]
GO
/****** Object:  Default [DF_tblIdTelField_Order]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelField] ADD  CONSTRAINT [DF_tblIdTelField_Order]  DEFAULT ((0)) FOR [Order]
GO
/****** Object:  Default [DF__tblIDTelG__msrep__01741E54]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIDTelGraphicDataImport] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblIdTelHistory_DateEdited]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelHistory] ADD  CONSTRAINT [DF_tblIdTelHistory_DateEdited]  DEFAULT (getdate()) FOR [DateEdited]
GO
/****** Object:  Default [DF_tblIdTelTemplate_CardFace]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblIdTelTemplate] ADD  CONSTRAINT [DF_tblIdTelTemplate_CardFace]  DEFAULT ((0)) FOR [CardFace]
GO
/****** Object:  Default [DF__tblImport__msrep__5F1F0650]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblImportErrors] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblISRoles_IsRoleBased]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblISRoles] ADD  CONSTRAINT [DF_tblISRoles_IsRoleBased]  DEFAULT (1) FOR [IsRoleBased]
GO
/****** Object:  Default [DF_tblISRoles_IsPopUp]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblISRoles] ADD  CONSTRAINT [DF_tblISRoles_IsPopUp]  DEFAULT (0) FOR [IsPopUp]
GO
/****** Object:  Default [DF_tblLeave_DateFrom]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblLeave] ADD  CONSTRAINT [DF_tblLeave_DateFrom]  DEFAULT (getdate()) FOR [DateFrom]
GO
/****** Object:  Default [DF_tblLeave_DateCreated]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblLeave] ADD  CONSTRAINT [DF_tblLeave_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF_tblMaxIDoftblTDI_LastUpdatedOn]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblMaxIDoftblTDI] ADD  CONSTRAINT [DF_tblMaxIDoftblTDI_LastUpdatedOn]  DEFAULT (getdate()) FOR [LastUpdatedOn]
GO
/****** Object:  Default [DF_tblMPTUsers_CreateEdit]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblMPTUsers] ADD  CONSTRAINT [DF_tblMPTUsers_CreateEdit]  DEFAULT (0) FOR [CreateEdit]
GO
/****** Object:  Default [DF_tblMPTUsers_SecFocalPoint]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblMPTUsers] ADD  CONSTRAINT [DF_tblMPTUsers_SecFocalPoint]  DEFAULT (0) FOR [SecFocalPoint]
GO
/****** Object:  Default [MSrepl_tran_version_default_1941634010]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblMPTUsers] ADD  CONSTRAINT [MSrepl_tran_version_default_1941634010]  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblPermission_Application_FK]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPermission] ADD  CONSTRAINT [DF_tblPermission_Application_FK]  DEFAULT (0) FOR [Application_FK]
GO
/****** Object:  Default [DF_tblPermission_IsSecured]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPermission] ADD  CONSTRAINT [DF_tblPermission_IsSecured]  DEFAULT (1) FOR [IsSecured]
GO
/****** Object:  Default [DF_tblPersonnelMaster_MissionCountry]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMaster] ADD  CONSTRAINT [DF_tblPersonnelMaster_MissionCountry]  DEFAULT ('Congo, Democratic Republic of') FOR [MissionCountry]
GO
/****** Object:  Default [DF_tblPersonnelMaster_CreatedOn]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMaster] ADD  CONSTRAINT [DF_tblPersonnelMaster_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_tblPersonnelMaster_IsRegistered]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMaster] ADD  CONSTRAINT [DF_tblPersonnelMaster_IsRegistered]  DEFAULT ((0)) FOR [IsRegistered]
GO
/****** Object:  Default [DF_tblPersonnelMaster_HID]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMaster] ADD  CONSTRAINT [DF_tblPersonnelMaster_HID]  DEFAULT ((212)) FOR [HID]
GO
/****** Object:  Default [DF_tblPersonnelMaster_DisplayMyMobile2_1]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMaster] ADD  CONSTRAINT [DF_tblPersonnelMaster_DisplayMyMobile2_1]  DEFAULT ((0)) FOR [DisplayMyMobile]
GO
/****** Object:  Default [DF_tblPersonnelMaster_HideTelDir]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMaster] ADD  CONSTRAINT [DF_tblPersonnelMaster_HideTelDir]  DEFAULT ('N') FOR [HideTelDir]
GO
/****** Object:  Default [DF_tblPersonnelMaster_Template_FK]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMaster] ADD  CONSTRAINT [DF_tblPersonnelMaster_Template_FK]  DEFAULT ((-1)) FOR [Template_FK]
GO
/****** Object:  Default [DF_tblPersonnelMaster_BackTemplate_FK]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMaster] ADD  CONSTRAINT [DF_tblPersonnelMaster_BackTemplate_FK]  DEFAULT ((76)) FOR [BackTemplate_FK]
GO
/****** Object:  Default [DF__tblPerson__msrep__4282C7A2]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMaster] ADD  CONSTRAINT [DF__tblPerson__msrep__4282C7A2]  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblPersonnelMasterISRoles_AccessibleFields]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPersonnelMasterISRoles] ADD  CONSTRAINT [DF_tblPersonnelMasterISRoles_AccessibleFields]  DEFAULT ('') FOR [AccessibleFields]
GO
/****** Object:  Default [DF__tblPrefix__msrep__2BCA4AD3]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblPrefixExt] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblRank_IsDefault]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblRank] ADD  CONSTRAINT [DF_tblRank_IsDefault]  DEFAULT (0) FOR [IsDefault]
GO
/****** Object:  Default [DF__tblRank__msrepl___371114F6]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblRank] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__tblSectio__msrep__6EC13C93]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblSectionHierachy] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblSections _SortValue]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblSections] ADD  CONSTRAINT [DF_tblSections _SortValue]  DEFAULT (0) FOR [SortValue]
GO
/****** Object:  Default [DF_tblTelDirStats_dt]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblTelDirStats] ADD  CONSTRAINT [DF_tblTelDirStats_dt]  DEFAULT (getdate()) FOR [dt]
GO
/****** Object:  Default [DF__tblTemp__msrepl___5ABA43E6]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblTemp] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__tblUserHe__msrep__503CB573]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblUserHelpDesk] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF_tblVehicleType_IsDefault]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblVehicleType] ADD  CONSTRAINT [DF_tblVehicleType_IsDefault]  DEFAULT (0) FOR [IsDefault]
GO
/****** Object:  Default [DF__tblVehicl__msrep__202DAF9E]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblVehicleType] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__tblWebSit__msrep__3C35BCC6]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tblWebSiteContent] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__tempCount__msrep__31B82E53]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[tempCounter] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__trunkingN__msrep__273A9FE0]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[trunkingNO] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
/****** Object:  Default [DF__VWUNADDRE__msrep__07C1F487]    Script Date: 01/28/2013 17:38:39 ******/
ALTER TABLE [dbo].[VWUNADDRESSBooKEmail] ADD  DEFAULT (newid()) FOR [msrepl_tran_version]
GO
