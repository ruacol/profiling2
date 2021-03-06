/****** Object:  Table [dbo].[SCR_ScreeningSupportStatus]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_ScreeningSupportStatus] ON
INSERT [dbo].[SCR_ScreeningSupportStatus] ([ScreeningSupportStatusID], [ScreeningSupportStatusName], [Archive], [Notes]) VALUES (1, N'Support Recommended', 0, NULL)
INSERT [dbo].[SCR_ScreeningSupportStatus] ([ScreeningSupportStatusID], [ScreeningSupportStatusName], [Archive], [Notes]) VALUES (2, N'Support Not Recommended', 0, NULL)
INSERT [dbo].[SCR_ScreeningSupportStatus] ([ScreeningSupportStatusID], [ScreeningSupportStatusName], [Archive], [Notes]) VALUES (3, N'Monitored Support', 0, NULL)
INSERT [dbo].[SCR_ScreeningSupportStatus] ([ScreeningSupportStatusID], [ScreeningSupportStatusName], [Archive], [Notes]) VALUES (4, N'Pending', 0, NULL)
SET IDENTITY_INSERT [dbo].[SCR_ScreeningSupportStatus] OFF
/****** Object:  Table [dbo].[SCR_ScreeningStatus]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_ScreeningStatus] ON
INSERT [dbo].[SCR_ScreeningStatus] ([ScreeningStatusID], [ScreeningStatusName], [Archive], [Notes]) VALUES (1, N'Added', 0, NULL)
INSERT [dbo].[SCR_ScreeningStatus] ([ScreeningStatusID], [ScreeningStatusName], [Archive], [Notes]) VALUES (2, N'Updated', 0, NULL)
SET IDENTITY_INSERT [dbo].[SCR_ScreeningStatus] OFF
/****** Object:  Table [dbo].[SCR_ScreeningResult]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_ScreeningResult] ON
INSERT [dbo].[SCR_ScreeningResult] ([ScreeningResultID], [ScreeningResultName], [Archive], [Notes]) VALUES (1, N'Green', 0, NULL)
INSERT [dbo].[SCR_ScreeningResult] ([ScreeningResultID], [ScreeningResultName], [Archive], [Notes]) VALUES (2, N'Yellow', 0, NULL)
INSERT [dbo].[SCR_ScreeningResult] ([ScreeningResultID], [ScreeningResultName], [Archive], [Notes]) VALUES (3, N'Red', 0, NULL)
INSERT [dbo].[SCR_ScreeningResult] ([ScreeningResultID], [ScreeningResultName], [Archive], [Notes]) VALUES (4, N'Pending', 0, NULL)
SET IDENTITY_INSERT [dbo].[SCR_ScreeningResult] OFF
/****** Object:  Table [dbo].[SCR_ScreeningEntity]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_ScreeningEntity] ON
INSERT [dbo].[SCR_ScreeningEntity] ([ScreeningEntityID], [ScreeningEntityName], [Archive], [Notes]) VALUES (1, N'JHRO', 0, N'Joint Human Rights Office')
INSERT [dbo].[SCR_ScreeningEntity] ([ScreeningEntityID], [ScreeningEntityName], [Archive], [Notes]) VALUES (2, N'JMAC', 0, N'Joint Mission Assessment Cell')
INSERT [dbo].[SCR_ScreeningEntity] ([ScreeningEntityID], [ScreeningEntityName], [Archive], [Notes]) VALUES (3, N'CPS', 0, N'Child Protection Section')
SET IDENTITY_INSERT [dbo].[SCR_ScreeningEntity] OFF
/****** Object:  Table [dbo].[SCR_RequestType]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_RequestType] ON
INSERT [dbo].[SCR_RequestType] ([RequestTypeID], [RequestTypeName], [Archive], [Notes]) VALUES (1, N'Joint Ops', 0, N'Joint operations with MONUSCO')
INSERT [dbo].[SCR_RequestType] ([RequestTypeID], [RequestTypeName], [Archive], [Notes]) VALUES (2, N'MDP', 0, NULL)
INSERT [dbo].[SCR_RequestType] ([RequestTypeID], [RequestTypeName], [Archive], [Notes]) VALUES (3, N'Transport', 0, N'Transportation provided by MONUSCO')
INSERT [dbo].[SCR_RequestType] ([RequestTypeID], [RequestTypeName], [Archive], [Notes]) VALUES (4, N'Training', 0, N'Training provided by MONUSCO')
INSERT [dbo].[SCR_RequestType] ([RequestTypeID], [RequestTypeName], [Archive], [Notes]) VALUES (5, N'Unknown', 0, N'Unknown request type')
SET IDENTITY_INSERT [dbo].[SCR_RequestType] OFF
/****** Object:  Table [dbo].[SCR_RequestStatus]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_RequestStatus] ON
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (1, N'Created', 0, N'Screening request has been created')
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (2, N'Sent to ODSRSG-RoL for validation', 0, N'Screening request has been created and sent to the ODSRSG-RoL for validation')
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (3, N'Sent to Conditionality Participants for screening', 0, N'Screening request has been validated and sent to Conditionality Participants (JHRO, JMAC, CPS) for screening input')
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (4, N'Sent to ODSRSG-RoL for consolidation of screening inputs', 0, N'Screening inputs have been provided by all Conditionality Participants (JHRO, JMAC, CPS), and request is now awaiting consolidation by ODSRSG-RoL')
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (8, N'Completed', 0, N'Screening request has been fully completed')
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (9, N'Deleted', 0, N'Screening request has been deleted')
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (10, N'Edited', 0, N'Screening request details have been modified')
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (11, N'Screening in progress', 0, N'Request has been screened by some of the conditionality participants but not all.')
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (12, N'Sent to SMG for final decision', 0, N'Screening request has been consolidated and is awaiting a final decision by the SMG')
INSERT [dbo].[SCR_RequestStatus] ([RequestStatusID], [RequestStatusName], [Archive], [Notes]) VALUES (13, N'Rejected', 0, N'Screening request has been rejected')
SET IDENTITY_INSERT [dbo].[SCR_RequestStatus] OFF
/****** Object:  Table [dbo].[SCR_RequestProposedPersonStatus]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_RequestProposedPersonStatus] ON
INSERT [dbo].[SCR_RequestProposedPersonStatus] ([RequestProposedPersonStatusID], [RequestProposedPersonStatusName], [Archive], [Notes]) VALUES (1, N'Proposed', 0, NULL)
INSERT [dbo].[SCR_RequestProposedPersonStatus] ([RequestProposedPersonStatusID], [RequestProposedPersonStatusName], [Archive], [Notes]) VALUES (2, N'Withdrawn', 0, NULL)
SET IDENTITY_INSERT [dbo].[SCR_RequestProposedPersonStatus] OFF
/****** Object:  Table [dbo].[SCR_RequestPersonStatus]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_RequestPersonStatus] ON
INSERT [dbo].[SCR_RequestPersonStatus] ([RequestPersonStatusID], [RequestPersonStatusName], [Archive], [Notes]) VALUES (1, N'Added to request', 0, NULL)
INSERT [dbo].[SCR_RequestPersonStatus] ([RequestPersonStatusID], [RequestPersonStatusName], [Archive], [Notes]) VALUES (2, N'Removed from request', 0, NULL)
INSERT [dbo].[SCR_RequestPersonStatus] ([RequestPersonStatusID], [RequestPersonStatusName], [Archive], [Notes]) VALUES (3, N'Nominated for PWG-Conditionality Meeting', 0, NULL)
INSERT [dbo].[SCR_RequestPersonStatus] ([RequestPersonStatusID], [RequestPersonStatusName], [Archive], [Notes]) VALUES (4, N'Nomination withdrawn from PWG-Conditionality Meeting', 0, NULL)
SET IDENTITY_INSERT [dbo].[SCR_RequestPersonStatus] OFF
/****** Object:  Table [dbo].[SCR_RequestEntity]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_RequestEntity] ON
INSERT [dbo].[SCR_RequestEntity] ([RequestEntityID], [RequestEntityName], [Archive], [Notes]) VALUES (1, N'Force', 0, N'Military component of MONUSCO')
INSERT [dbo].[SCR_RequestEntity] ([RequestEntityID], [RequestEntityName], [Archive], [Notes]) VALUES (2, N'PNC', 0, N'Police Nationale Congolaise')
INSERT [dbo].[SCR_RequestEntity] ([RequestEntityID], [RequestEntityName], [Archive], [Notes]) VALUES (3, N'FARDC', 0, N'Forces Armées de la République Démocratique du Congo')
INSERT [dbo].[SCR_RequestEntity] ([RequestEntityID], [RequestEntityName], [Archive], [Notes]) VALUES (4, N'UNPOL', 0, N'Police component of MONUSCO')
INSERT [dbo].[SCR_RequestEntity] ([RequestEntityID], [RequestEntityName], [Archive], [Notes]) VALUES (5, N'Unknown', 0, N'Unknown requesting entity')
INSERT [dbo].[SCR_RequestEntity] ([RequestEntityID], [RequestEntityName], [Archive], [Notes]) VALUES (6, N'UNDP', 0, N'Imported from Sygeco')
SET IDENTITY_INSERT [dbo].[SCR_RequestEntity] OFF
/****** Object:  Table [dbo].[SCR_RequestAttachmentStatus]    Script Date: 01/23/2013 10:52:20 ******/
SET IDENTITY_INSERT [dbo].[SCR_RequestAttachmentStatus] ON
INSERT [dbo].[SCR_RequestAttachmentStatus] ([RequestAttachmentStatusID], [RequestAttachmentStatusName], [Archive], [Notes]) VALUES (1, N'Added to request', 0, NULL)
INSERT [dbo].[SCR_RequestAttachmentStatus] ([RequestAttachmentStatusID], [RequestAttachmentStatusName], [Archive], [Notes]) VALUES (2, N'Removed from request', 0, NULL)
SET IDENTITY_INSERT [dbo].[SCR_RequestAttachmentStatus] OFF
