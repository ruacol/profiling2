using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201306191648)]
    public class RequestRespondBy : Migration
    {
        public override void Down()
        {
            Alter.Column("RespondBy").OnTable("SCR_Request").AsString(255).Nullable();
            Execute.Sql("UPDATE SCR_Request SET RespondBy = OldRespondBy");
            Delete.Column("OldRespondBy").FromTable("SCR_Request");
            Delete.Column("RespondImmediately").FromTable("SCR_Request");
        }

        public override void Up()
        {
            Create.Column("RespondImmediately").OnTable("SCR_Request").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.Column("OldRespondBy").OnTable("SCR_Request").AsString(255).Nullable();
            Execute.Sql("UPDATE SCR_Request SET OldRespondBy = RespondBy");
            Execute.Sql("UPDATE SCR_Request SET RespondBy = NULL");
            Alter.Column("RespondBy").OnTable("SCR_Request").AsDateTime().Nullable();

            Execute.Sql("UPDATE SCR_Request_AUD SET RespondBy = NULL");
            Alter.Column("RespondBy").OnTable("SCR_Request_AUD").AsDateTime().Nullable();
            Create.Column("RespondImmediately").OnTable("SCR_Request_AUD").AsBoolean().Nullable();

            Execute.Sql(@"
                UPDATE SCR_Request SET RespondImmediately = 1
                WHERE OldRespondBy LIKE '%immediate%' OR OldRespondBy LIKE '%asap%' OR OldRespondBy LIKE '%urgent%' OR OldRespondBy LIKE '%as soon as possible%';

                UPDATE SCR_Request SET RespondBy = '2012-06-19' WHERE RequestID = 149;
                UPDATE SCR_Request SET RespondBy = '2012-07-27' WHERE RequestID = 160;
                UPDATE SCR_Request SET RespondBy = '2012-07-10' WHERE RequestID IN (167,169,170);
                UPDATE SCR_Request SET RespondBy = '2012-08-01' WHERE RequestID = 179;
                UPDATE SCR_Request SET RespondBy = '2012-07-18' WHERE RequestID = 181;
                UPDATE SCR_Request SET RespondBy = '2012-07-19' WHERE RequestID IN (183, 184);
                UPDATE SCR_Request SET RespondBy = '2012-07-24' WHERE RequestID = 185;
                UPDATE SCR_Request SET RespondBy = '2012-07-27' WHERE RequestID = 186;
                UPDATE SCR_Request SET RespondBy = '2012-07-24' WHERE RequestID = 187;
                UPDATE SCR_Request SET RespondBy = '2012-07-30' WHERE RequestID = 189;
                UPDATE SCR_Request SET RespondBy = '2012-08-03' WHERE RequestID = 190;
                UPDATE SCR_Request SET RespondBy = '2012-08-10' WHERE RequestID = 191;
                UPDATE SCR_Request SET RespondBy = '2012-06-19' WHERE RequestID = 195;
                UPDATE SCR_Request SET RespondBy = '2012-08-31' WHERE RequestID = 196;
                UPDATE SCR_Request SET RespondBy = '2012-08-24' WHERE RequestID = 197;
                UPDATE SCR_Request SET RespondBy = '2012-08-22' WHERE RequestID = 198;
                UPDATE SCR_Request SET RespondBy = '2012-08-27' WHERE RequestID = 199;
                UPDATE SCR_Request SET RespondBy = '2012-09-03' WHERE RequestID = 206;
                UPDATE SCR_Request SET RespondBy = '2012-09-07' WHERE RequestID = 207;
                UPDATE SCR_Request SET RespondBy = '2012-10-10' WHERE RequestID = 209;
                UPDATE SCR_Request SET RespondBy = '2012-09-07' WHERE RequestID IN (210,211,214);
                UPDATE SCR_Request SET RespondBy = '2012-10-01' WHERE RequestID = 212;
                UPDATE SCR_Request SET RespondBy = '2019-10-01' WHERE RequestID = 213;
                UPDATE SCR_Request SET RespondBy = '2012-10-10' WHERE RequestID = 216;
                UPDATE SCR_Request SET RespondBy = '2012-09-30' WHERE RequestID IN (217,218,219,222,227);
                UPDATE SCR_Request SET RespondBy = '2012-09-07' WHERE RequestID IN (220,221);
                UPDATE SCR_Request SET RespondBy = '2012-09-15' WHERE RequestID = 223;
                UPDATE SCR_Request SET RespondBy = '2012-09-17' WHERE RequestID IN (225,226);
                UPDATE SCR_Request SET RespondBy = '2012-09-24' WHERE RequestID = 229;
                UPDATE SCR_Request SET RespondBy = '2012-09-26' WHERE RequestID = 230;
                UPDATE SCR_Request SET RespondBy = '2012-09-28' WHERE RequestID = 231;
                UPDATE SCR_Request SET RespondBy = '2012-10-01' WHERE RequestID = 232;
                UPDATE SCR_Request SET RespondBy = '2012-10-04' WHERE RequestID = 233;
                UPDATE SCR_Request SET RespondBy = '2012-10-05' WHERE RequestID = 234;
                UPDATE SCR_Request SET RespondBy = '2012-10-03' WHERE RequestID = 235;
                UPDATE SCR_Request SET RespondBy = '2012-10-10' WHERE RequestID = 238;
                UPDATE SCR_Request SET RespondBy = '2012-10-11' WHERE RequestID = 239;
                UPDATE SCR_Request SET RespondBy = '2012-10-15' WHERE RequestID IN (240,241);
                UPDATE SCR_Request SET RespondBy = '2012-10-20' WHERE RequestID = 243;
                UPDATE SCR_Request SET RespondBy = '2012-10-31' WHERE RequestID = 244;
                UPDATE SCR_Request SET RespondBy = '2012-10-28' WHERE RequestID = 245;
                UPDATE SCR_Request SET RespondBy = '2012-11-15' WHERE RequestID = 246;
                UPDATE SCR_Request SET RespondBy = '2012-10-31' WHERE RequestID = 247;
                UPDATE SCR_Request SET RespondBy = '2012-11-05' WHERE RequestID = 248;
                UPDATE SCR_Request SET RespondBy = '2012-11-06' WHERE RequestID IN (249,253,254);
                UPDATE SCR_Request SET RespondBy = '2012-11-09' WHERE RequestID = 255;
                UPDATE SCR_Request SET RespondBy = '2012-11-08' WHERE RequestID IN (256,257);
                UPDATE SCR_Request SET RespondBy = '2012-11-18' WHERE RequestID IN (260,261);
                UPDATE SCR_Request SET RespondBy = '2012-11-26' WHERE RequestID = 262;
                UPDATE SCR_Request SET RespondBy = '2012-11-20' WHERE RequestID = 263;
                UPDATE SCR_Request SET RespondBy = '2012-11-27' WHERE RequestID IN (264,265,267,268);
                UPDATE SCR_Request SET RespondBy = '2012-11-28' WHERE RequestID = 269;
                UPDATE SCR_Request SET RespondBy = '2012-12-03' WHERE RequestID = 272;
                UPDATE SCR_Request SET RespondBy = '2012-12-05' WHERE RequestID = 273;
                UPDATE SCR_Request SET RespondBy = '2012-12-30' WHERE RequestID = 274;
                UPDATE SCR_Request SET RespondBy = '2012-12-20' WHERE RequestID = 275;
                UPDATE SCR_Request SET RespondBy = '2012-12-13' WHERE RequestID IN (279,280);
                UPDATE SCR_Request SET RespondBy = '2012-12-12' WHERE RequestID = 281;
                UPDATE SCR_Request SET RespondBy = '2012-12-14' WHERE RequestID = 282;
                UPDATE SCR_Request SET RespondBy = '2012-12-17' WHERE RequestID IN (283,284,287);
                UPDATE SCR_Request SET RespondBy = '2012-12-24' WHERE RequestID IN (286,290);
                UPDATE SCR_Request SET RespondBy = '2012-12-18' WHERE RequestID = 288;
                UPDATE SCR_Request SET RespondBy = '2012-12-25' WHERE RequestID = 289;
                UPDATE SCR_Request SET RespondBy = '2012-12-27' WHERE RequestID = 291;
                UPDATE SCR_Request SET RespondBy = '2013-01-08' WHERE RequestID = 295;
                UPDATE SCR_Request SET RespondBy = '2013-01-03' WHERE RequestID IN (296,297);
                UPDATE SCR_Request SET RespondBy = '2013-01-16' WHERE RequestID = 298;
                UPDATE SCR_Request SET RespondBy = '2013-01-10' WHERE RequestID = 313;
                UPDATE SCR_Request SET RespondBy = '2013-01-14' WHERE RequestID IN (317,319,321);
                UPDATE SCR_Request SET RespondBy = '2012-01-14' WHERE RequestID = 318;
                UPDATE SCR_Request SET RespondBy = '2013-01-17' WHERE RequestID = 325;
                UPDATE SCR_Request SET RespondBy = '2013-01-26' WHERE RequestID IN (326,328);
                UPDATE SCR_Request SET RespondBy = '2012-01-30' WHERE RequestID = 330;
                UPDATE SCR_Request SET RespondBy = '2013-02-01' WHERE RequestID IN (331,332,335);
                UPDATE SCR_Request SET RespondBy = '2013-02-06' WHERE RequestID = 336;
                UPDATE SCR_Request SET RespondBy = '2013-02-08' WHERE RequestID = 337;
                UPDATE SCR_Request SET RespondBy = '2013-03-07' WHERE RequestID = 342;
                UPDATE SCR_Request SET RespondBy = '2013-02-27' WHERE RequestID IN (343,344);
                UPDATE SCR_Request SET RespondBy = '2013-03-15' WHERE RequestID IN (348,352);
                UPDATE SCR_Request SET RespondBy = '2013-03-22' WHERE RequestID = 353;
                UPDATE SCR_Request SET RespondBy = '2013-03-21' WHERE RequestID = 356;
                UPDATE SCR_Request SET RespondBy = '2013-03-26' WHERE RequestID IN (357,358);
                UPDATE SCR_Request SET RespondBy = '2013-03-22' WHERE RequestID = 359;
                UPDATE SCR_Request SET RespondBy = '2013-03-28' WHERE RequestID = 360;
                UPDATE SCR_Request SET RespondBy = '2013-04-02' WHERE RequestID IN (363,366);
                UPDATE SCR_Request SET RespondBy = '2013-04-01' WHERE RequestID = 364;
                UPDATE SCR_Request SET RespondBy = '2013-04-22' WHERE RequestID IN (374,376);
                UPDATE SCR_Request SET RespondBy = '2013-04-26' WHERE RequestID = 375;
                UPDATE SCR_Request SET RespondBy = '2013-04-20' WHERE RequestID = 377;
                UPDATE SCR_Request SET RespondBy = '2013-04-04' WHERE RequestID = 379;
                UPDATE SCR_Request SET RespondBy = '2013-04-09' WHERE RequestID = 380;
                UPDATE SCR_Request SET RespondBy = '2013-04-25' WHERE RequestID = 381;
                UPDATE SCR_Request SET RespondBy = '2013-04-29' WHERE RequestID = 385;
                UPDATE SCR_Request SET RespondBy = '2013-05-04' WHERE RequestID = 386;
                UPDATE SCR_Request SET RespondBy = '2013-05-03' WHERE RequestID = 388;
                UPDATE SCR_Request SET RespondBy = '2013-05-08' WHERE RequestID = 389;
                UPDATE SCR_Request SET RespondBy = '2013-05-07' WHERE RequestID = 391;
                UPDATE SCR_Request SET RespondBy = '2013-05-15' WHERE RequestID IN (392,393);
                UPDATE SCR_Request SET RespondBy = '2013-05-20' WHERE RequestID = 395;
                UPDATE SCR_Request SET RespondBy = '2013-05-21' WHERE RequestID = 396;
                UPDATE SCR_Request SET RespondBy = '2013-05-24' WHERE RequestID = 397;
                UPDATE SCR_Request SET RespondBy = '2013-05-26' WHERE RequestID = 398;
                UPDATE SCR_Request SET RespondBy = '2013-05-27' WHERE RequestID = 399;
                UPDATE SCR_Request SET RespondBy = '2013-06-01' WHERE RequestID IN (400,401,406,407,409);
                UPDATE SCR_Request SET RespondBy = '2013-05-31' WHERE RequestID IN (402,405);
                UPDATE SCR_Request SET RespondBy = '2013-05-30' WHERE RequestID IN (403,404);
                UPDATE SCR_Request SET RespondBy = '2013-06-03' WHERE RequestID = 408;
                UPDATE SCR_Request SET RespondBy = '2013-06-04' WHERE RequestID = 410;
                UPDATE SCR_Request SET RespondBy = '2013-06-08' WHERE RequestID IN (411,412);
                UPDATE SCR_Request SET RespondBy = '2013-06-15' WHERE RequestID = 416;
                UPDATE SCR_Request SET RespondBy = '2013-06-27' WHERE RequestID = 417;
            ");
        }
    }
}
