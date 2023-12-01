﻿#define FEWER_ARTISTS
using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;


namespace AirtableApiClient.Tests
{
    // The class Artist is used to abstract all fields of a record of the 'Artists'
    // table. In this process we need to make sure that all Airtable field names are
    // valid C# field names by making use of the [JsonProperty("AirtableFieldName"] 
    // annotation; see JsonProperty("On Display?")] below as an example.
    public class Artist
    {
        public string Name { get; set;}
        public List<AirtableAttachment> Attachments { get; set;}
        public string Bio { get; set;}

        [JsonPropertyName("On Display?")]
        public bool OnDisplay { get; set;}

        public List<string> Collection { get; set;}
        public List<string> Genre { get; set;}
    }


    [TestClass]
    public class AtApiClientTests
    {
        static readonly string TABLE_NAME = ":/?#[]@!$&'()*+,;= Fewer Artists\\ %2F -._~";
        readonly string AL_HELD_RECORD_ID = "rec6vpnCofe2OZiwi";
        readonly string AL_HELD_NAME_FIELD_ID = "fldSAUw6qVy9NzXzF";
        readonly string AL_HELD_COLLECTION_FIELD_ID = "fldE0muAk6ejOkkKa";
        readonly string MIYA_ANDO_RECORD_ID = "recTGgsutSNKCHyUS";
        readonly string EDVARD_MUNCH_RECORD_ID = "recaaJrI2JbRgEX5O";
        readonly string TABLE_ID = "tblUmsH10MkIMMGYP";

        const string APPLICATION_ID = "app1234567890ABCD";                          // fake app id for tests
        const string API_KEY = "key1234567890ABCD";                                 // fake airtable api key for tests
        readonly string BASE_URL = $"https://api.airtable.com/v0/{APPLICATION_ID}/{Uri.EscapeDataString(TABLE_NAME)}";

        static private AirtableBase airtableBase;
        private FakeResponseHandler fakeResponseHandler;
        private HttpResponseMessage fakeResponse;

        //private readonly string UrlHead = "https://api.airtable.com/v0/";
        private readonly string UrlHeadWebhooks = "https://api.airtable.com/v0/" + ("bases/" + APPLICATION_ID + "/webhooks");

        readonly string NOTIFICATION_URL = "https://httpbin.org/post";
        //readonly string BAD_NOTIFICATION_URL = "https://httpbin.org/bad_post";

        [TestInitialize]
        public void TestInitialize()
        {
            fakeResponseHandler = new FakeResponseHandler();
            airtableBase = new AirtableBase(API_KEY, APPLICATION_ID, fakeResponseHandler);
            airtableBase.ShouldNotRetryIfRateLimited = false;
            airtableBase.RetryDelayMillisecondsIfRateLimited = 2000;
            fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            if (airtableBase != null)
            {
                airtableBase.Dispose();
                }
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TaAtApiClientListRecordsTest
        // List records
        // Returned records do not include any fields with "empty" values, e.g. "", [], or false.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TaAtApiClientListRecordsTest()
        { 
            // Rules: Escape all '"' to '\"' and all '\n' to '\\n' and also delete extra spaces infront of the first ' {' and after the last the '} ' in fakeResponse.Content.
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Rg2fyrhyOY7GgfVgxWKrzw/VUJBpVVHbYerPJDqMlwSbazYjOjGj3V0bgQAmJhwylDOxk_alQWSX0aenvpVmSv_/aDvPW3z12IIxHtXE02AkFNaHc-EkYBmnuw_Cau_dce8\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ASex5Wk5JI_xNW-OplUtOg/B1hcdklZ2-xZJ_qhX-hdamY5Q7OUfPnvFZ2m8wOcbomC9B-eEwPHEl5DEm781PLvyrfnrhrwnXxaTMXy_Z-0pg/yPBQ6urWOiAsUDMYRJfD-6AnSLglmnvGSSC6t37GzhM\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/e9bonJd4ZfIV9by4S4rMBw/c7DzbQajt_nZhzCTXr2fAnlWKlvHdvHKPpjDt5jcRATzHUa3Gz85xsvr7BCUxo8dQRH1XZ2RkTwGgcDpxkF5Hg/jfP7jAhBPCZ-gyajl8_ECgWrOnitmQaFW0uDaCmMDJc\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/4l-OGLt6I6JIkXj5XUIlOA/s8BfAiG7cN4HM4roDaQeIbhWue-TC4VSk9s31X8E33Hk_pOuM8-Cq4yB6i3rLQBm5kzZn9Q2eCUZa9CUxNG78w/YVqTGmK-XujFiBpNDkgzOza88KAZesvpsckcdCZRFC4\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/AuqkVRbpAZb7GuOg4rDlmQ/r_3wICWEjrGPkq0w9omYA4oDQL-uD56FcsxSB1nv-4JhApFQhepl5AbS5wpevVcK/fmMK6wTxR1obhkVg5S9i3l8eaemgAm1nS1l19qK65FY\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/f6Mv1eTmr9alenvp7N8mHQ/kVDrhuNHYT83S3EsNzgd33yonw6fyAYp7bmSap7AmaFDP0FMZ3j_QW7lID_d1ksg/NON7aoK4F_hd-GZn6uNtsSEwGs00fKeBa-_F6PNEkTs\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/pd_kOQr2338vbng-M8zE9g/hwObapLuecTja22eXsxbGhq1xuQSvborj_4wqvMsypXceZKdrWzBtC3tFwAQ7qni/P507eHSN4jkpESvRgW_qgInScP0DN3n6gbnho_tpiIc\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ZHflD6Fg8apdG9vf-IKAAg/8ZUji3w-ujotEglH_1QrRy82xjNEUuKwAY38R4autY4ECmAr0eNotiJRUZEmRlnr/n88U9vM8EM21swyQw6iceJT5wY-ntVnAsN8WRNPj3qE\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/z2rjYhBYNK7tE_D4Mzb-Cg/HUBfADTgF6_Hwlg47bG67-Lr0vEWt5HorgpvlKSmARsIYvJDc4l0C4SyuwZ0-2yd/ab2dS_-lj7bWwV7JBiBSVOQYBuHrAQiyTmNmu2a6LJ0\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/-0dRRhUthQkSRIzKl4NEeA/NDwCkEA4noM3lVgvQuz4JwXyjIbUUbNxResik_GNVnqJkQsZyHbXDcjNSx8YJduA/wg-2dsHccXux8Qh2ghd2bnIxRTQZgIlXin6JhwkruFM\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/gGtQSG07RXo_WgPWdD48Xg/FdhEbarH4meoJRimEkGQ8YVpPhxHiPUTtW3hYZLbrzj9H1PgSpNonXJ2i8z68Cc_/mCnEUkFyjMmjthyFQTkiqWGxIojeUsdSYwlk6CNmmbY\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/djATfwCeAYwPFnHEaROmIA/pdyEjlJe7P6p3mZLvNhHVfIOAhjAwV2NCUvonQ8tybfzd6pAGj0gF_UyZNn-8PD7/a5b5HtXU4zoYQdnlKdzQRSLhVmSoI2HxJxcWwaFAYxs\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/50dymLq3LzLT9cLrUDQ2hg/l04pe9fi0NZzuQbLO0_7bcedfo4AhGwFpguf4loksIw-Al3agyNhqkDvn876XZl2/CRzRgjcH0vZozQgy9B7FOkxrdd14c2zw1JPjZny9OOM\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/bo43KchTTNlL5j7c3ge2cQ/AK90EtMtCzpheGBgfOU4puqsht821hRetmuI_lQO5YwSRJDh8RRG9licXYMjSbHb/KVeSropOm3tO_V5AITQyXMVYL9eb0Boz8szkn63abJo\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/pgEJBRQbCUEPMuUQvkCR9g/XBv35TMsdT49lSLG2ZEGW9Nn6IeO0FNeRuDbnfAHeXqE-5opQzvAcd6DtW-45-FY/8E-kG0TxQLxm7ArQDgzVCIVxt3FPtA5paUwZPRSehZ4\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/PQ9KQwykPriMgLLgVOBP2A/TcQr3BuIdZIaVVL8rHq0Ey8GH5y0CYrKVMWEqjv5Cx1b8BEjFa2qNz57p8_XFixJ/ksiOFktLqFzoZRyct42ASoWl75w29iPDqoHOBKxtCWk\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Lq0bWryDJJFiYVTGLclOKA/4c-l__0rp9CwIMWqUVjzRbR3pHX-q0nuIAeecM6wK_5NYaxgK1_i6-bLM-VeDKMd/wmlJdTImxKggkM_-gHpFCbxp_U9TYs-fPCftja0rJPY\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/7rt2MlnjjryWDztZoQuoyA/28x34cF99hOtHW_ecLIcNBaPIb2c3pPS2GFeLL_fZjB6ratpA339tRSr2KFg1Tuj/-DLb5dzlrb6laEdVfpoY_rM4Qqwll5ubqBcEPMvonE8\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/x23aVPARIOMQRx2bk0HjJw/yBVpz_e3sbs0CRBoBfYXjrJOVKvRqNf3dfierNm0XJu2ixWtIufBu4NK2d7FwcZL/-kGwxer8jy_7V51NppsN6Oj0T6gNPE6wJfXXM66YeI4\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/OlvK2rEay8xeVayOd_Dt2Q/gPD9mRuSN9e3aEDr1ik7ebETO51hRyUG_k9R0RXUJpRQ_7v67SxGI5gJb4lOwtNF/w3Mlxla_yFet76FDrGj9CLnNbWXelbj4qcPNxcxiQew\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/A8VGi06XX5nClf-gR34btw/pdNXKFmbmeNqWGAE8nTUqcaW-JoR7FUFuTdJKtXkXJtZPv2ju5IWdY0mYYylXFNzbe_w-jvQud9lVwrZNM4Kow/1prscCLVOI6fuwFYnOsdq24pK8Itk3_1QE9BE3p7gn0\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/VOU4rOdKTNxQ5s7l9DhTSw/Rlx28O_bWcBEB8CTyaeXqEt2ATdDMYY4DXSqiLvPil4_0J9VgbMCY-QYS_SWRbOjszy-rt-xtASlNdPwiaXEgQ/B3rm-SR4kmMmFyR6qr2hYPwUvtRRqKltBpJrqA_Ga9k\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/An7q_4--wFWIKY9CA536uw/9-yafSJ3khohNBoxw1qUfG3OkJ06BV9XKRhiGz0BD0RD-L1Nh3PQ2W--h2jJQt56w08q2TGS5tL16J59ltCysA/D5_5EWSrt9DM-gHgnxzIhV2K4WKDvGSjYwCrQ9-A1y8\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/zTHVmAHBNhUJdU53nNnzkw/MXGtccykVCgwnqMM5wqjvO1x2T72qjgmkXxxfCAkIEdm79Su44SR-8DdYpwdUZ5bJhziGHDDRRM6FA_LP5SNwQ/9ZqhheuoSkDEjppu8N3V95pDWy_bHSsu1FSkLmp7meM\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/b4AHeD-UFEYGZC-A3WHCaA/aZE6FyGTCo1eMuemC3EIUIm8ecqNeYbqGyCIGyWb5gP3bzFlUiFjTbEFE5vG_Qm3/c7ubXMbSrpCq5WwM4Hyd2hhkQxkiTmilXNbVp7g-Yho\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/gjll_Rgf-yxIL5AhgIXn5w/HNUNEC9swDlU8hljlJ5qN3lxqWF_4rzZO3sNduV0xliFo5vBtYUtCKEkwtYOr1HvdyYklj2ZRdjltvrnciw4Xg/vWQvr5jM9rVWd3DUzx5mqadRkboJKtlooVyBZdygyns\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/V2Fsgmb1tn_h84Nyp_y3Qg/eJD1N9-uQFqdV8EvX7MkiEaPoc7tygs6Cc6aIeTr_XUgD3YiB6FlCHjPkWTuNufA_KKmfz5T0ZEjG1ytB4jc8Q/oh_ZxJt0eK1GTC9qh0jRSaKo9vPVmHwGR5SvvaCndwA\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/PtLCPXZa2DDQCHtz_ExRZQ/Oxn9vsRPL3wJ0IYYp7eSVuygmAsULkMcU9KRK7NxQCNyGSNa7biNxLGgteU14leR-GuDnMaInkjQmjEM1ahkMQ/gZA34TDswNXw6S0WQV3PIC-rVVLaeZ--SuLmA_O5B80\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/N6OJC2N_kXoZoFMNmEsHOw/MaSFg-Cw_YyJFkZXpYCwjGA_WoUB-AZ9kiUYsXLRyo-y6raWbT6f70t_GUSu4tY7/YV7lCP8hOUJYFSOgf8q3HQc-uoV22EKGvgzLhN6DJV8\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/AxwcH-oEeprEloJumUyuYw/75oXsZzgp54vsF_6IsiTmpy4V0iz1MhPctAPqZzkzXVjlp8phOMyGFcMWBsWVuUW/rkSThFHzE1GDotrY2gWxaTdEJ-SrIVSM4HdoVEze_W0\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/6iENCVWZjTbUIAur7xaA3g/9CWHMHV0cj--HE1IlBYiklWkGm4iytkBtqzRD5oB7A7tMptRKK3dyP0m7iM3kBAi/CvCFAPuZE9oRDIkdbdyTuPsWdgCUkDKahSMKgHcEnFY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/p_tAxLRPO6Qzcw7sXgsANA/UIAb9OXXo0T9JyXDE73q6r0Hjpo96pCf_DJS6GC_Wu1ZVt1NFAjSEuw6nFytkzWO/wyHMeRmTazixlYL3P83ZN93AH7NegFJt7npqW06XNfE\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/iH7GW5bg7CpkoxDIyvmc_g/aiYRcT-5Rav3giGETZVB1xGeIzyuBQ8z8qhhEwH2Dr1g75Q44YKFVt7qLTr6WkukFssy70QKe-dsyW3ohHZejQ/k8eV_UROKNi9WKhjPIlIVM8hXES0UQSXEq9-ids4icA\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/9q9DLJGI0v-LSlt3hqtiwA/0GISsnoK6V_RXnN_0AoJ8b3RT2NZrIPts-8jUU_LhKZ_oRNsA3N8HiC5YSg-_gWaQ8MENruUuJI7cue-Jk2ZcA/EeZlYeZPs7mDEvLQhPZpmmUlyT0bzLGH8Kj5pNBM-1A\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ixNwNZP7uE1-LBVNaLVyXg/vCMq4wSSU7HeDZgz3HUVAnjU5MmhBfPWBf8dK656VgMxJN1Qu1XREIWsjevrv13AaPX_xjYcfXc3rYQ3JHZ_IQ/jDEcMRynSE1a0wgPutLvwZM9Qr7wqVnqKhe_6e7DdyI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/3AvJaZTK8d1qQz0CErSIxQ/FZC2mYfGtIJ1q8uNLSC_1TGrJo8MwLAqNNc-2r_xCufLNFvWsD_Et4PH_oIxRRxafdE8HCdM7z129Fs6oQM5fA/v6v9dzJtsTSvtlFKe4DFAABrIrG_skor4te36ie0MMA\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/CbLHYrCuNzKXM7r-Q1M4GA/6JgD_HwsfPIt81Ur5pSK1bMlUO33jFI6t_LbjXh5k8Fmd70zHoqzU1YMWv1FOD4-sBn93sN7hi1YyrlDDaqbEWCGQFrztXLaZx2EDNLLHZK4PURHljrciMgWO6ceqKylYgOGIowSPim4lBSWu-nlhw/WpNUeRDf9rHrc-FFpSmmff5Ki9XVQUcqFJmybCQJw2Y\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/2bzEqHq-E0f5WLEspPnEhA/_Z-wvZ0HqvaVQldbFks3YkXNKoBNMgycqwldlcQtQwohtlF1y4bPdbd4Fu8_-ISCmqx0VHbh3FdDJr4V79OjnndD1BG3dLhGbMcdgJi1RZtQ4nDmkTQigKVMyoNPNaOYtc9t1U1M-rrXKlGjVHCfbQ/wPg6zEiK14VTvaKICPIxnZNWPcbrSW0vQWd1hcHAQC8\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/9xz9DTK6aqnyh98v6NztPQ/xlPT5LYig876kKQvP2TAVuYZy_nM0Yzpzdbu3zNN2vqujMRLz6yxA1NTDGdNypQKsh1aVkSAcD4gdTV8_XcTFYA5RtBsVQF0mPEHp0jwPW1Q-6D0w4vi_RqvkqUx6Dx04dvhkOlN8Xm4YvzhNIaP-g/GD1QZlfG2nA2r-d9-v7vlFu90lMI-ffAILNZ7EDtwcM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Ntu5o9q2US5KGpanVF1rYg/Z30QvtPcX7XfrlnDBBnN7tHUOfDZxkYDlf-8hJPOs0N7uZP6HhRTxtnBVMNPPAtroHVk2XKbVf2a1g64naLQn3C7ZfbMkbWSybpNR1qEa6hQdfZnyql39Ed2mG5-TVvSlAQHvOIEak1_0gvme_aKSQ/wKX4p272K8izj9dbrebuGyZ6qHtZk40l5yVz9OBAU0Y\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/3DrF1L8JTjn8n-lEwKrRbA/buZfndDvL-hNJ6SP9-vBgphGzp5C0nC0rwN-696bDINxECuZOy6XNVMiOT21ck8hW9xM0ChDQ8PB37yfPnSmNRdk2LBesJgi_jrHsXt011wzg5d7pSu04cnyxJxvMBqWng-WrNMIu1wUNvjJY_Bkw2ZhyZ-mreYFbm3oS9CDVqq5naPmd1MB2Slq9dvmF0WN/sNH_VH2Tar4bwqAd3okbCIM3MLrmJIpGx-AgEanps-Q\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Qz6ISsLo1v_BOJyBn2VjTg/Fr6M5T4wL2epDJT5_SfvezS2D6zKHfKsa0AUm4iQFmaBHkufd61y_EGC7F21tKcMO9G3TA8gYe1laleDG6tcrCrfETwT6DEG0HyyBpot3GSWe3NE0t2XPyWW8J2vLpyWLJCbH737MafXneA3IfNWvLk9KDcrUMqjahDAxr1JWufOmUOfF93_NsKmiE3_BqWx/A-3SX_DlAIbfIt6WG61SOXhJDSGEEFtiE8tmUSEmK7Q\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/32ELoAAy9su1DllcsCpqYA/R2fUP-usQDDtXkmTaVvddP-lysy2zj9VDI0zTWIPE8L_GGbs9E4PQXIRgcXcTiCeZdU0SgaNMzM0ww6GLCCmI588QoMUYnZyfEkDjgLPM0z0GXBEfiA52m4JMwFj7Nqk0XLFJhKHl8pNLdG0RFdfBRt4bQAXgjLyqspyNhKYxBnh8Bsm09zM7MSam3iSu3Ar/2k6_iYIxfGku1gPOL746h0IBbqirVlCjcwxBc11-EeM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/5oWf_7KZB_msiwCqQsBemQ/2y7LuPmYQ8jeIF3Lc0kobynL334qlX37hoA-mu70OwyjSNl0MxWPZO6IK0XBzlROLZIebGf81oRKZ6veSlWFrc4z6DgfcN5FBBkrFaj6Gc5dVelOcXsSygCOJY1tlWhDdvmxncbXADom2qYBLWHWjlOYpkyKWUEPM5cV_G2K_CWYf77weEP2Mod2hn54dRVI/yBDzG1tYV3xmkFKMcNXvn4tTnEEePHdK59Y-uKA-SIQ\",\"width\":600,\"height\":600}}}]}},{\"id\":\"recaaJrI2JbRgEX5O\",\"createdTime\":\"2015-02-10T00:15:45.000Z\",\"fields\":{\"Genre\":[\"Abstract Expressionism\",\"Modern art\",\"Surrealism\"],\"Bio\":\"Edvard Munch was a Norwegian painter and printmaker whose intensely evocative treatment of psychological themes built upon some of the main tenets of late 19th-century Symbolism and greatly influenced German Expressionism in the early 20th century. One of his most well-known works is The Scream of 1893.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Edvard Munch\",\"Testing Date\":\"2012-02-01T23:36:00.000Z\",\"Collection\":[\"recwpd7MLPQqorfcj\"],\"Attachments\":[{\"id\":\"attNIEYhExe4q53lp\",\"width\":1000,\"height\":579,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/1WdEkAT2z5_JS0_h5FJiIw/dn97Usxt_4gbR0qhwbEayNnYbd1FFL8BdtYgWmZO1d1UDYg7qyC-_RlmaLqVBQE9/76T-Nu1kzqkkFWOQ1Kkj8leNyqJF-HSAUtrf7pByeMQ\",\"filename\":\"The_Sun.jpg\",\"size\":194051,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/5G1DdxWPzsHbzWdvfpwzIg/nMKCgmc5j0JaZttQChQuQnjyGxzKqFfAJQM_vzXqbxpax6iVhkRDK2jodpOAotMK/_-XEyxHtWylA3kuCFXY-eSqI3OBCS0rV---3PA1ST30\",\"width\":62,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/O5s02IhWnfY0d6lPASMSRw/kx49z73IvBs1x8HrX6b2lQKI7u07QcCpWvOpw04IeieQUAg4aej5pNBVJ4NPnRyA/ghl8qs6XdBT1zKsnpKllbgmHw4ORiQFa2PDyVmGC4pQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Fehp585nO-dCrKRhBix7xA/GoBauLy577lNdLKQ3E6H32taVMGW1rG7ydiAClLD3t18MVLgtpJrr9MezZc2t17C/FWiemyFveAoELOOqrMiHzHD69o5QOTccVbRRTmrBH4w\",\"width\":1000,\"height\":579}}},{\"id\":\"attVjzN5X8xdWoc2W\",\"width\":1458,\"height\":1500,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/grCQxB9Ud78PbS7WKDPrKA/9pd5v-ZdEDYV7nUVvphtZTLJXd0rQ8SQe4zPzr1uzRIy03NkmIi17LXFK6IswavAja9H8-vq-xKqG5jsgK91yg/TfzbPtupLc8j5X4AqTONzTMPwNMNke4RxHRGmVvjNi4\",\"filename\":\"Munch_Det_Syke_Barn_1896.jpg\",\"size\":425603,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/lxUwVgWlPplpv15Re5Apkg/4sRtr2fVCtK9JAaQviyR07852BTdFkfyCO3-NUttCXM5CFaa7WRPd9xAAoXkLmI1o9wTtfYjCJ_gecKJAXekjA/OhMOWY7ss4x6aicHXTQT-hdNi-Daz5APAs7Yq3GVU6w\",\"width\":35,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/aGNN-5i2hyuYaPlDvdlENA/Q42_DG_DOy_H_m27QlUaw327jNW_HlJhU-Rqfm-t38LHyIuy93G6AMeRl3OXjqKmvvlrTjRGDUcxMlks_1IaTA/RiwxTJPheuZPU3sTvpph225ZY4FiDq3-Gf4xC1GcPt8\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/5ftDmMiSRwzIVu9ZLBkFiA/sgcnOOS7cvFruYfVFKYmm2UxhKlI8_fjD17jF2DLF9s9R9sAOcv8500PQn4yaR5N4w0zukp6n9Qkfu2Y3ut4MA/bpXzu9hQhOe-5ZmuySbsZhEKd9x1Re6KmwOu7fS_kOE\",\"width\":1458,\"height\":1500}}},{\"id\":\"attnTOZBfCiHQhfy1\",\"width\":850,\"height\":765,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ZI8lfqsQVFJdmcj28sidlg/yR9d6HDyWjsq2Eih90iJQtHWwFzEzkWIDhUP0gO7MJeIoUTYfgsQRZMhDvhvdoo9/65OOvLP0rCEIDAeeBtHekORXnGUXjd7MsZJSh8H-Ejo\",\"filename\":\"death-in-the-sickroom.jpg\",\"size\":255101,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/B2E04Jt_OkrUDHVimE-jcg/db_E4wcNomL1XR1q3W7GjZxl-4-y2u_TRuZOWsTAagL-ibcPurZbruCxiNGZcXKf0OssLFb9AC2-K3q527kYHw/4spjv2ZG57AjhyYLAi_-icw93qAjujfW4ruGxy-HKz0\",\"width\":40,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/7HH2jUkug_GY8blfZHz_tg/NTci6mwSso44aiXgWhc9aaPhygxYgxCg4pxzElblLYH87JpwrX6BkHwTavxVDxWnzYFjKBCZVoue4wCIYCgCtg/IE8bW2ZBgghaJUKHpXbjPJJTojN89SKW9Kg-9XKCRR8\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/-Uhpmd1nZHtKM3hDZci_fQ/NVjGeAMYEy26QXI_f06411WmXnerqhZQmpUd3dWhmAMQoY2mjlNlNDqK6O_tkD0HORWK5EcOZW-2C1w-8zsVvQ/ctECfBYO_ZglWwxAVaBLGTW14gId1Xg17QBnYm3diVs\",\"width\":850,\"height\":765}}}]}},{\"id\":\"recj31Rc5TXAiVZV3\",\"createdTime\":\"2015-02-09T23:36:53.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Experimental Sculpture\"],\"Bio\":\"Isamu Noguchi (野口 勇) was a prominent Japanese American artist and landscape architect whose artistic career spanned six decades, from the 1920s onward. Known for his sculpture and public works, Noguchi also designed stage sets for various Martha Graham productions, and several mass-produced lamps and furniture pieces, some of which are still manufactured and sold.\",\"Name\":\"Isamu Noguchi\",\"Collection\":[\"reccV1ddwIspBOe4O\"],\"Attachments\":[{\"id\":\"attuoGtQSGoeWEurX\",\"width\":640,\"height\":487,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/fvGFSc5esmX3ckujnKYsig/FXMwLniIRG_BKLT6ULjnxzc7-_pgJmYpOs4ctHf7NXU/eMalDZWlQ4GCj12PvctGctaRvKQr2Y8gyCbJSmtEgSQ\",\"filename\":\"Leda.jpeg\",\"size\":55738,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/CNvm2QLk2RR-cKnBUXwONg/8CX4UxTJ-yOQNghlM48YqsCgeUTQJk5az8KELVdwvEmqrNkrQZ7yaqbxE2_vJZZ7/JCijpUwXNWbYOonRTQO-9Wxy0kfmiKvHnPcRs1oqYD8\",\"width\":47,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/G8RZLJ8c48dtoEhaJuHyvg/QW3l_V-tkIxp_2dnFH05psd8bC_GDHCQH7gYh1WfFdi7r-wTzjMzY2Bdo1b_XM6t/V2n8pErY36oHhTluMsPUaSfRcLz9WC-bDJHj12Mel5E\",\"width\":512,\"height\":487},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/w1XIxjj77YHJQQ30aK5ZIw/hDEVu4T7Suny6TuJg0EzOT9KhiN-hwgRokjM-Ia2Ql-YEtxlbkRKfu5kQsNnq0tU/TG9m0IoEyCTDWvcqZDfx-yDOCZhFabgAbvi-1VNL7Fk\",\"width\":640,\"height\":487}}},{\"id\":\"att4MPT0gu8r2DZdv\",\"width\":414,\"height\":526,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/An9RA05IiRLpj979V3sJpQ/ixiW6qg35woRK-MBEHOFZsMVrpgKeLNKl4orB1owSBjpULiAOfxaahml38fSJOco/xAlc5RVB4zIMtNZ3RrtbX9mUwAOM5_U37beOKxhdQgY\",\"filename\":\"Mother_and_child.jpg\",\"size\":38679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ryGJTmT4PuLVpuWPkYVkLg/6aF8pVm9rA7tSlm9ig_EttegrphUPX7xqDq0F1cI3CO7QnQ_umSNtDT5vD-gnVwp/nieu2ez6mgC75KwPw993d2oEAIPIPg4f4KXMzg1waD0\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/7YHio0dWXpmEIkWMZ6XL9w/4eBhffzTD21psF6SgEWD0-iEm_dMwPZrRAcNHiPal5p1WthoBanipu4PPly2iqjf/jlydV0-RW3WJX8tId0nHgXSm5GlRghJUBn0y-z4BylY\",\"width\":414,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/RJEw5GNM6EpqtRLkDzAW1A/mVRkixAHvaH95L73k6VKVZjZXAUEr5v4LZa52Z-0nU1kUZn9dPWrXcN4PU46eDYM/7zMZV4ESAkBT3R2f2FfiPU28DLfzGBtA2gwwoGsiHEE\",\"width\":414,\"height\":526}}},{\"id\":\"attsGNtljepdSppj3\",\"width\":3694,\"height\":2916,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ooF6XjITxWnaP2miWpk9XQ/Cpvcqy-hjoC4R45lxrLPu5olrfRzgdbwGJoPLvgkO8KND8Qf5YCQN07GIYWHaXot/Rkjvo4L4jmCqyFHtztdk5oqnbbrP4H3p7buA-oOhrv8\",\"filename\":\"Sky-gate.jpg\",\"size\":10002210,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/-f4a8Pv2_GcVdB1W1LNDLw/9FIppgsEyNaimR2-zovLo-jIiFneEUi6cVwkDjqmn-8KqH-a_HtjDsstX_l0mjfN/xGvGLFjJ9bXTjNaU3lpXTyLH5GPe4mhZo-Ipw637-eU\",\"width\":46,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/w51WuA-dws_54YgDEWrjXA/AIks5Dgn0yOQFkAXWC5MVW1aZdaUQcTI7INdfB3Zji4XhjHU2Rzp2pgOz1viOyZj/nWbAwih4Tj0Udj3eowZocRBf_LZf7ZbiyC0Mnqo0FBo\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/e2KzkBv6-PIOKx1XZrGl1Q/tXcSUeJ17Xj6y2GrQK9Gbw4XhFnm9NZaksCkdn1FIjljwdjEteD4SsXlVDBCw-vs/MWszvuJQ8h7e8obse2_EMORdD5Mz8bF8aD54GSQ8rLw\",\"width\":3000,\"height\":2368}}},{\"id\":\"attpYKQ4gldei2tWd\",\"width\":960,\"height\":696,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/sOC6Da1WyFqtV7MWX2TFvw/4tJpL75XESkE9FkjPhy36BpCMoyM6QDkTMn08tee6kBVRQI0Y8hdigRAvQqPhfUO/O7ZPwkk8s7ZK93JzVhbR5TLA2gj1kNGCuJ9GaXlksYs\",\"filename\":\"Akari_Lamps.jpg\",\"size\":110954,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/RFdvkbISmnaEGDLDcpIc_A/IihYIsA8II3uKOacv1fMB00Vn1HfZCh_voBoiKCTuKNBnuB-hOE8ES3jVVZekAFm/LXy0Gzo88WlaDjX1v3u_CCdL8suaATI_k6x_XXFIBSw\",\"width\":50,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/JIePB3C3ZWwegi71wCDbAA/URw3LRuOzRqCyDjUyBNYyxiZ1_X6Rqnu2kjGxjFHSPOlMTH_DRxhI0IveLgKXul3/X_hrhXUN2V-hSJ_zAe36gF80zNxJOOdxxsDOoNlBWhA\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/yxclQvM7-Qz_t572sapOdg/pYu_-pGsWd--4MyAnAr9Ag0gCPjYuPF8FKfldGNK73pr5ISVy3gRwhi8R-yyaKDM/Oi7vXzgUCuhPQ7YmKKHqlbALL4NDDApmp--PookWAko\",\"width\":960,\"height\":696}}}]}},{\"id\":\"recneNPDcZsNQDxsb\",\"createdTime\":\"2015-02-09T23:28:08.000Z\",\"fields\":{\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Thornton Willis is an American abstract painter. He has contributed to the New York School of painting since the late 1960s. Viewed as a member of the Third Generation of American Abstract Expressionists, his work is associated with Abstract Expressionism, Lyrical Abstraction, Process Art, Postminimalism, Bio-morphic Cubism (a term he coined) and Color Field painting.\",\"Name\":\"Thornton Willis\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"att8enrlgYD3FiHXB\",\"width\":433,\"height\":550,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/B9AqFBb169hiKRN5FGH4xQ/N_iatPkzLmHo4MJFn8UjXNI_QKrbiRPXT7C4ehP6PL8SPRqLdU4oNtnhSthugEsT/0UCyO2WaVzPQvSF7nbLX8ymiQLzUPXNz50sSerWbav0\",\"filename\":\"Color-Drawing.jpg\",\"size\":374784,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/dNMQzIIm8NurBNmjmSmgpA/ruPrah8ZOl3J5lDUX5HTcpjJVQ-9pvB0zHEKcCI-d3ynw-fWX5SrP03PNym5EZwL/Ysx-1A14uvIniTRSuWawwVO5TwlgJAx3UWs8Ve5t22A\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/smMm_pBlHkbJea2uVBZ9lg/aNWmgi_94sPnwfUX5Tj5qoX34nJ5ss-03xgkRL4SuTnvS6tfL4u9othXDEJTpRL4/hcTB0fub7krb6m2m6_NVwLm9vbP5zf52bT6XCj6N37Q\",\"width\":433,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Q-NEfQnLW6YjBMSuPKSGyw/THRn3RrQgqOfve8zwFlHIsvG51EhoWKOC7EWChjtfRZbJt3RWHFhTRXA3OFtXI_Q/EnDkpd8zOv8KRj2gEKXsPCH2Z3UW2XJsESnszLkPS_g\",\"width\":433,\"height\":550}}},{\"id\":\"attqEXWllptvGjPDQ\",\"width\":551,\"height\":700,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Zr1hLi5sLTtGoJROqzpN-Q/KXy_xcMe0gcxMowy9LFHXhiiJw0_HC3_LF5leIIkc_pea6s5OWsVUBDKzjDQZxED/bcTLlO7XKnIBl2WcBCRLdBM-IMEUN_mW3_4gskNcGuc\",\"filename\":\"study_No._2.jpg\",\"size\":109204,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Nd5k0Eduo-Z_N_UA8Fq_yQ/aJVcRIjl1bQK9lvJby9G_7y5bxnuqgmaI6XdZfZNq3lWqJNjA4fpO7XlOA1hYkoR/fzFivK7grsOc_tt03pNWIpFCBv38AjvB4fkb86lMXY0\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/0N8z7AvrCXNiI1iCVU81_Q/O5fHUNMLYj53d-aPWGEZLiUphnrsIneRYH5kU_eZ5paoZYAFO-FiYQacfOEH-KUG/7xPWKkL-CONSmTy9vhXDmLfVwhffs2PI7pj8Qbniu-8\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/QFbikLJPpBCfHCAExdNBJw/tzEfApmyr6BhTsCUgnT1GZOMXHsn6xYd4m4CdwWMPUGiFiC00tQAxTcBp-CP8IPa/fd1et_N6uNc6rUJlm24KUjXxedabsJcsNW9p-Lu-mA8\",\"width\":551,\"height\":700}}},{\"id\":\"attHI8lwWwEoooJ24\",\"width\":890,\"height\":1200,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/_-pjCt9ombNs2P1PbtJ0kw/J6SkmE-qlzjpOfUqbRgmsB_EDNJikD6I6VQkB7wlTFN6EPND8D8pdw8206i6pvMA/s7QcKSLcBOlGjWTfQD8dn4wQUP7Ouf7XXa-qUGyWVSw\",\"filename\":\"three_gray_squares.jpg\",\"size\":109091,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/eWUk3o4QkNUHIKoZHPH32Q/7IyqBthtDbVt93oblh9Kd7QpZIIwEZ74YbWntLToGq3hse0ReP7TKvyYv7laNmizOHyJlu1FODecjcYQmslKmw/_AMdBgCWhxwcvUAgwYf0AaoBCOit7oqa8VjcqwRDBGM\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/08rGEowY0qBl29SWuVxgwQ/7QXXEe9lYA10g8cNm_NndcRbxh9D4nSvEy3lBpAUWP41dITaY30fGqmwEWUeovKcL-VSDaQj79G-GM0cWvn-YA/loVsVh-z3KwwIwbju1J-vFazH7Fnwp6EItEs6Gnake4\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/RYedyuu_9pygC0E6fbCnPA/ZegrToyFoBIJ13ADnTSh63e1CK-tE6GTZ8iFf-WVDPFb0um5b5yA4Ngkbt9ZqvhG-npbNCTSyK3S32G2qdABsw/9OUzOy1hdn1-KrsINef-cFyVUctX4FFVmVw1zHLm-h0\",\"width\":890,\"height\":1200}}},{\"id\":\"att5TIJ00ppiNYQm3\",\"width\":639,\"height\":517,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/z98ACbOS2CJNqKn5OrKe4g/ZheLCh4R0RZ-O6V-DGz7-poVC8PcpNkohw7nXDIeJoJGPw50_UxxE0aFziQ4evAi/Tb87x3DOVNL-9NW7O5VakfimOf6S9rV5_NfIQPUNpkY\",\"filename\":\"Streets_of_Tupelo.jpg\",\"size\":27664,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/wbwjkOnc7-UTUD3A-ZXzbw/nwFBhWgDNd6ytzvGGfbHkrbrLLeNQh0_oHSrKdLaCd4Ivy_zps6xPxVDNjU9mRmY4EwMT9HkcGTNoUdYibiw6Q/dpjp1Ig1LItdal4Dnv_YcvtJaAfOi9h1pV9yqK8I2x4\",\"width\":44,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/KncA9IjVHWhN7YpYFLit8w/F2dETomdhHUOxDkwHT-SiDnUcA2kKfZVOTfDTFRWw8mmPjzOWndJAJaVIWfB4SJdeP23uTgkEycJX-KdGbOUug/g4mMuc2h2KQg3GMADOn5jKKVDcso9OBp1iI7M-LOpBE\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/-SBNlo5CvsmAQKLfr8Or7A/d0lEWlKjtxpoIpy1i2QPfB-ay3zCqLu_sTG7mKXuHfnu3Vn5hgT6yW1e4Buufuwb/-oV4j04g_0KFaAbOcORnvttePclECalIJWvsxu6LnKI\",\"width\":639,\"height\":517}}}]}},{\"id\":\"recraBPRF3m5Te5Hn\",\"createdTime\":\"2015-02-09T23:24:10.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Mark Rothko is generally identified as an Abstract Expressionist. With Jackson Pollock and Willem de Kooning, he is one of the most famous postwar American artists.\\\\\\\\\\n\",\"Name\":\"Mark Rothko\",\"Testing Date\":\"2020-07-06T17:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attONu0jXlWNlHOxh\",\"width\":213,\"height\":260,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/j0EBinTxUiJc9a56D-3Tfw/wxuKNdGzzwlPLFKIiAjtfQWkyn9YmCOIj6NRBg5-6oCbXdl5_HuUTEzMgb9t_VUO/0v0s9lc0p92dhvqmMNX66uyTg0m-CsTT_0GWXYiaoik\",\"filename\":\"No._18_1951.jpg\",\"size\":7416,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/TKFbKKiFIU21qMvWSmd0pw/-WBVrpTTCGUyvSGjQY1X3i3Awbfg8FhgviBc23WKx7-desK5d03mxa2imXRk2C5d/3oM31Xqffof_LIL3OQMNHf1PaFu63jQRApH_9Kej21w\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/IPuihQEH2oTtFBinQdTxEA/qKnAi6AdvsUhmd1BjmW8MBe1PJEUIfp_et-v0G4x3QNvOTTuUNZaO6o3qCMITL-Q/DVDvS5JArSMXxnF1N9ici--PvX5zA5BL5vvivdWxiDA\",\"width\":213,\"height\":260},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Cb__gLij2rYejxHCyGKczg/pyDsHArSDAYlnG-Z1xqH5WBJ14SJuXouEdKi2uSioDKxNTbre45m343TTWti6dr0/oESqHv6lURMnMpMp21oq6jGk7B20Hh4U52oBQJBYIMQ\",\"width\":213,\"height\":260}}},{\"id\":\"atteYo0fXP5bOpxt6\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/RRgThEw65cvHSl6LWcCGfw/8CS0zxulL8Ao0k8DPMt0DPOGBHkFXjPP4zo2fxrrENmrQB_452smll-LKEkZ3SC-/V08JMga2TYsPvY9gFt-PohfXUFwmby45VRUYDBEf36A\",\"filename\":\"Untitled_1954_RISD.jpg\",\"size\":23636,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/fOAxz_EJuPIrvq_DmpmC_w/tK8Xw0ADNoZSNnQaEm6js7w4s9mRh7Z3cKo8cC2V0A_cRms66RCM94FgcTqvV2kLmbAn1FEacUNr7nvnT8CXng/UiGNvdeB0mGUFoim6o1UlFOKnbxK6sSuamQBeyEpuJI\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/jq1c82fmz9A2JhlMbUQfJA/XN_2N0CoBfEnzTXZdM2gDr2j27P0OWBJchER4xaTXjr6iChcG8pqeUFqf7M4ERu3QxGUERl0LTYPt0GyL-4sMw/i3eGUmwJJSJwzg6v8aDyKqxJQpzOqtBXdmg35hw5Qu8\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Or9_Nsx4Br-RNqNm6SBJ2w/ozC6501wXukHZnXU9IZLTF_caQp7IH2X3P9jbpA5321bgoBHwjJbcgNA3NN1jrVy4LcIW_0wiOCTGWD2mrPezw/jzzLo2IwientFnE9DssWEJOF994pQ4qWwfMN47My0ho\",\"width\":385,\"height\":640}}},{\"id\":\"attneJn9hR5DtDns9\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/PKu-F2K9j01SuVUoQMHJHQ/0RHgbIdzPrr6v6BDc7SdbSWpducJpP9oOQJIwmUWw9lA4zOc3rTfKcIZ4asSA30u/5TsIgqdUtaMdun3a84gmvM08QrbChuiIoqXLmp84-94\",\"filename\":\"Untitled_1954.jpg\",\"size\":33352,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/rfWy1Uy98OaNnWAHn4J1eg/ze0lESdxrNHiGQKXBwuKRD1BAMheKjmHxA5tY7YVTsCUiz1hF_5z0oZ9tvPSCp7D/GTUPUsk6TdrAdwnGhOPORYsYZvfH1mKks3rTvovqedQ\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/KP28P5fUgbJwWW0HJ38y7Q/khp5mkgmDvitufJGPW5FmfTmX3ajn9LEEXF-FBXTtSOLNKd2YBbkaI_NOyxDmM6W/EpR72OftmSIaH1JRakPfvOiBDQRm5nNeVynvzpH6W5c\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/3ScPM82cdj4nyT7V4M3Zng/uZ2Y9O-2AfFMYe_U0TkvyoiHfAy5Ubp7CX8GY3_lx_krW9WeJXluifqzVsQfOxFy/dE66llE2YYYmV0Mc_XCsXYBi9cVemVnmmfXDMCrsEwU\",\"width\":385,\"height\":640}}},{\"id\":\"attnicjT3NIYNL5Le\",\"width\":551,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/aEz7F-okgVf0GAjXGPsf1A/I0uew1WRiPAlT0C6zsM2aV9ravVdhF33fy0iCQZeY_2xKKiiTUMACQIviRbh2lh1/slbOSGvtwyFAI-EcdBfm48nLjXy-IH1UrYqHHdpLCQ8\",\"filename\":\"Untitled_(Red,_Orange).jpg\",\"size\":43346,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/7IM32RAEuWSZ0QAty2CerA/0puIqwBPdeNM-FuDb21fWztkgzHtWY2XSw58i9x8hfFyTMic8U0tlSCtWRO6OMazk87ISERLSYGduql6rZov-Q/4tVT9JS6xx4pQllSnKrUWf40Br75lBq3rbyrFp4ZL8Y\",\"width\":31,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Yh5XvCkrhQbKbmpHDXCBdQ/FNkEffELjSm-20aEV3nxfnnbTwj2dqOpwVu-x9fpR4-1T64EtoWDFTUyiq4COaL7kr3TjHCWPxGePlhPV3u1pA/M_whgyqQVkCYJlYcwcReJZjDaBfzwojd7IDbKeR24EI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/IY6p4NiaswTPO8epa9krTQ/Lu39XgK0Ga0Ag2WjOF0qCNvr_aMeeS7Zzvr33WB08j0HNHmVNih4TNNk99bL6iLutiCZwh7JeLPm4YP3zhQB7g/IATnl_4L4iEQLY6XGr3sfB5dTwWY73unaxg2dXmJ2EE\",\"width\":551,\"height\":640}}},{\"id\":\"attpDkpjaf734NjM0\",\"width\":480,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/B0fDOn85JRVX9pUm1seWEA/Sw9maCap6Hu6Z2gyncdIrZhGotc0mMKhSfDL646DrIYVvNNDraJqq40_lXCkyyeU/s8NJH9kEZQF7lcA2Qe0pK5shvcH5jI8OB7mzVSVUnsI\",\"filename\":\"No._61_(Rust_and_Blue).jpg\",\"size\":35819,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/YnIBPog9J_djh2beBBHrtQ/Yxs-Au-eXjKcaQvaNfT05LsA0sVJdOZyCVgomFHskXN4Lkie_AOMAA09fboz1kqk9bKE8q_M0cyImEOzAxQKwQ/-aUgsbDMwmbhN_jhBAOzDgOLauGMpGOMoQWtTI3bD5I\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/zVPaRquPQB-uP-yFwtECeA/Jm2MRA_tNJM8oTChTkLviqKtf-CP3xYZma3qfrf3MmZGfSKBAn1Gj14khKC2QTKGrjjcwgGKln4ICrAnD2BJ_A/5z1Jw6xlFNZtlVctvsSQWnTxm7mMOdFoBsU-vD5Mfyk\",\"width\":480,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ZAeBmEw74_Ch1esVC8NqKQ/sgsxCn7dW2l414G9iPMI-Mt0GXEA5fuBHdrXF33fS2YGFZjMZYjtMUJgBrPR1xUPe2OVw4co8qk-t4BAumSwXQ/jI-sWwCf1gC5JhKYFTE3bSXAkPKWOy0wKTXcPXhk7LY\",\"width\":480,\"height\":600}}}]}}]}");
            
            string bodyText = 
                "{\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/listRecords",
                    HttpMethod.Post,
                    fakeResponse,
                    bodyText);

            Task<ListAllRecordsTestResponse> task = ListAllRecords();
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count > 0);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TbAtApiClientListRecordsTest_Template
        // List records where the fields of the record are deserialized to type <T>.
        // <T> is <Artist> in this test.
        // The returnFieldsByFieldId flag (false by default) should not be enabled when using template.
        // Returned records do not include any fields with "empty" values, e.g. "", [], or false.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TbAtApiClientListRecordsTest_Template()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Rg2fyrhyOY7GgfVgxWKrzw/VUJBpVVHbYerPJDqMlwSbazYjOjGj3V0bgQAmJhwylDOxk_alQWSX0aenvpVmSv_/aDvPW3z12IIxHtXE02AkFNaHc-EkYBmnuw_Cau_dce8\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ASex5Wk5JI_xNW-OplUtOg/B1hcdklZ2-xZJ_qhX-hdamY5Q7OUfPnvFZ2m8wOcbomC9B-eEwPHEl5DEm781PLvyrfnrhrwnXxaTMXy_Z-0pg/yPBQ6urWOiAsUDMYRJfD-6AnSLglmnvGSSC6t37GzhM\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/e9bonJd4ZfIV9by4S4rMBw/c7DzbQajt_nZhzCTXr2fAnlWKlvHdvHKPpjDt5jcRATzHUa3Gz85xsvr7BCUxo8dQRH1XZ2RkTwGgcDpxkF5Hg/jfP7jAhBPCZ-gyajl8_ECgWrOnitmQaFW0uDaCmMDJc\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/4l-OGLt6I6JIkXj5XUIlOA/s8BfAiG7cN4HM4roDaQeIbhWue-TC4VSk9s31X8E33Hk_pOuM8-Cq4yB6i3rLQBm5kzZn9Q2eCUZa9CUxNG78w/YVqTGmK-XujFiBpNDkgzOza88KAZesvpsckcdCZRFC4\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/AuqkVRbpAZb7GuOg4rDlmQ/r_3wICWEjrGPkq0w9omYA4oDQL-uD56FcsxSB1nv-4JhApFQhepl5AbS5wpevVcK/fmMK6wTxR1obhkVg5S9i3l8eaemgAm1nS1l19qK65FY\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/f6Mv1eTmr9alenvp7N8mHQ/kVDrhuNHYT83S3EsNzgd33yonw6fyAYp7bmSap7AmaFDP0FMZ3j_QW7lID_d1ksg/NON7aoK4F_hd-GZn6uNtsSEwGs00fKeBa-_F6PNEkTs\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/pd_kOQr2338vbng-M8zE9g/hwObapLuecTja22eXsxbGhq1xuQSvborj_4wqvMsypXceZKdrWzBtC3tFwAQ7qni/P507eHSN4jkpESvRgW_qgInScP0DN3n6gbnho_tpiIc\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ZHflD6Fg8apdG9vf-IKAAg/8ZUji3w-ujotEglH_1QrRy82xjNEUuKwAY38R4autY4ECmAr0eNotiJRUZEmRlnr/n88U9vM8EM21swyQw6iceJT5wY-ntVnAsN8WRNPj3qE\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/z2rjYhBYNK7tE_D4Mzb-Cg/HUBfADTgF6_Hwlg47bG67-Lr0vEWt5HorgpvlKSmARsIYvJDc4l0C4SyuwZ0-2yd/ab2dS_-lj7bWwV7JBiBSVOQYBuHrAQiyTmNmu2a6LJ0\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/-0dRRhUthQkSRIzKl4NEeA/NDwCkEA4noM3lVgvQuz4JwXyjIbUUbNxResik_GNVnqJkQsZyHbXDcjNSx8YJduA/wg-2dsHccXux8Qh2ghd2bnIxRTQZgIlXin6JhwkruFM\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/gGtQSG07RXo_WgPWdD48Xg/FdhEbarH4meoJRimEkGQ8YVpPhxHiPUTtW3hYZLbrzj9H1PgSpNonXJ2i8z68Cc_/mCnEUkFyjMmjthyFQTkiqWGxIojeUsdSYwlk6CNmmbY\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/djATfwCeAYwPFnHEaROmIA/pdyEjlJe7P6p3mZLvNhHVfIOAhjAwV2NCUvonQ8tybfzd6pAGj0gF_UyZNn-8PD7/a5b5HtXU4zoYQdnlKdzQRSLhVmSoI2HxJxcWwaFAYxs\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/50dymLq3LzLT9cLrUDQ2hg/l04pe9fi0NZzuQbLO0_7bcedfo4AhGwFpguf4loksIw-Al3agyNhqkDvn876XZl2/CRzRgjcH0vZozQgy9B7FOkxrdd14c2zw1JPjZny9OOM\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/bo43KchTTNlL5j7c3ge2cQ/AK90EtMtCzpheGBgfOU4puqsht821hRetmuI_lQO5YwSRJDh8RRG9licXYMjSbHb/KVeSropOm3tO_V5AITQyXMVYL9eb0Boz8szkn63abJo\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/pgEJBRQbCUEPMuUQvkCR9g/XBv35TMsdT49lSLG2ZEGW9Nn6IeO0FNeRuDbnfAHeXqE-5opQzvAcd6DtW-45-FY/8E-kG0TxQLxm7ArQDgzVCIVxt3FPtA5paUwZPRSehZ4\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/PQ9KQwykPriMgLLgVOBP2A/TcQr3BuIdZIaVVL8rHq0Ey8GH5y0CYrKVMWEqjv5Cx1b8BEjFa2qNz57p8_XFixJ/ksiOFktLqFzoZRyct42ASoWl75w29iPDqoHOBKxtCWk\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Lq0bWryDJJFiYVTGLclOKA/4c-l__0rp9CwIMWqUVjzRbR3pHX-q0nuIAeecM6wK_5NYaxgK1_i6-bLM-VeDKMd/wmlJdTImxKggkM_-gHpFCbxp_U9TYs-fPCftja0rJPY\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/7rt2MlnjjryWDztZoQuoyA/28x34cF99hOtHW_ecLIcNBaPIb2c3pPS2GFeLL_fZjB6ratpA339tRSr2KFg1Tuj/-DLb5dzlrb6laEdVfpoY_rM4Qqwll5ubqBcEPMvonE8\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/x23aVPARIOMQRx2bk0HjJw/yBVpz_e3sbs0CRBoBfYXjrJOVKvRqNf3dfierNm0XJu2ixWtIufBu4NK2d7FwcZL/-kGwxer8jy_7V51NppsN6Oj0T6gNPE6wJfXXM66YeI4\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/OlvK2rEay8xeVayOd_Dt2Q/gPD9mRuSN9e3aEDr1ik7ebETO51hRyUG_k9R0RXUJpRQ_7v67SxGI5gJb4lOwtNF/w3Mlxla_yFet76FDrGj9CLnNbWXelbj4qcPNxcxiQew\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/A8VGi06XX5nClf-gR34btw/pdNXKFmbmeNqWGAE8nTUqcaW-JoR7FUFuTdJKtXkXJtZPv2ju5IWdY0mYYylXFNzbe_w-jvQud9lVwrZNM4Kow/1prscCLVOI6fuwFYnOsdq24pK8Itk3_1QE9BE3p7gn0\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/VOU4rOdKTNxQ5s7l9DhTSw/Rlx28O_bWcBEB8CTyaeXqEt2ATdDMYY4DXSqiLvPil4_0J9VgbMCY-QYS_SWRbOjszy-rt-xtASlNdPwiaXEgQ/B3rm-SR4kmMmFyR6qr2hYPwUvtRRqKltBpJrqA_Ga9k\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/An7q_4--wFWIKY9CA536uw/9-yafSJ3khohNBoxw1qUfG3OkJ06BV9XKRhiGz0BD0RD-L1Nh3PQ2W--h2jJQt56w08q2TGS5tL16J59ltCysA/D5_5EWSrt9DM-gHgnxzIhV2K4WKDvGSjYwCrQ9-A1y8\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/zTHVmAHBNhUJdU53nNnzkw/MXGtccykVCgwnqMM5wqjvO1x2T72qjgmkXxxfCAkIEdm79Su44SR-8DdYpwdUZ5bJhziGHDDRRM6FA_LP5SNwQ/9ZqhheuoSkDEjppu8N3V95pDWy_bHSsu1FSkLmp7meM\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/b4AHeD-UFEYGZC-A3WHCaA/aZE6FyGTCo1eMuemC3EIUIm8ecqNeYbqGyCIGyWb5gP3bzFlUiFjTbEFE5vG_Qm3/c7ubXMbSrpCq5WwM4Hyd2hhkQxkiTmilXNbVp7g-Yho\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/gjll_Rgf-yxIL5AhgIXn5w/HNUNEC9swDlU8hljlJ5qN3lxqWF_4rzZO3sNduV0xliFo5vBtYUtCKEkwtYOr1HvdyYklj2ZRdjltvrnciw4Xg/vWQvr5jM9rVWd3DUzx5mqadRkboJKtlooVyBZdygyns\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/V2Fsgmb1tn_h84Nyp_y3Qg/eJD1N9-uQFqdV8EvX7MkiEaPoc7tygs6Cc6aIeTr_XUgD3YiB6FlCHjPkWTuNufA_KKmfz5T0ZEjG1ytB4jc8Q/oh_ZxJt0eK1GTC9qh0jRSaKo9vPVmHwGR5SvvaCndwA\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/PtLCPXZa2DDQCHtz_ExRZQ/Oxn9vsRPL3wJ0IYYp7eSVuygmAsULkMcU9KRK7NxQCNyGSNa7biNxLGgteU14leR-GuDnMaInkjQmjEM1ahkMQ/gZA34TDswNXw6S0WQV3PIC-rVVLaeZ--SuLmA_O5B80\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/N6OJC2N_kXoZoFMNmEsHOw/MaSFg-Cw_YyJFkZXpYCwjGA_WoUB-AZ9kiUYsXLRyo-y6raWbT6f70t_GUSu4tY7/YV7lCP8hOUJYFSOgf8q3HQc-uoV22EKGvgzLhN6DJV8\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/AxwcH-oEeprEloJumUyuYw/75oXsZzgp54vsF_6IsiTmpy4V0iz1MhPctAPqZzkzXVjlp8phOMyGFcMWBsWVuUW/rkSThFHzE1GDotrY2gWxaTdEJ-SrIVSM4HdoVEze_W0\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/6iENCVWZjTbUIAur7xaA3g/9CWHMHV0cj--HE1IlBYiklWkGm4iytkBtqzRD5oB7A7tMptRKK3dyP0m7iM3kBAi/CvCFAPuZE9oRDIkdbdyTuPsWdgCUkDKahSMKgHcEnFY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/p_tAxLRPO6Qzcw7sXgsANA/UIAb9OXXo0T9JyXDE73q6r0Hjpo96pCf_DJS6GC_Wu1ZVt1NFAjSEuw6nFytkzWO/wyHMeRmTazixlYL3P83ZN93AH7NegFJt7npqW06XNfE\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/iH7GW5bg7CpkoxDIyvmc_g/aiYRcT-5Rav3giGETZVB1xGeIzyuBQ8z8qhhEwH2Dr1g75Q44YKFVt7qLTr6WkukFssy70QKe-dsyW3ohHZejQ/k8eV_UROKNi9WKhjPIlIVM8hXES0UQSXEq9-ids4icA\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/9q9DLJGI0v-LSlt3hqtiwA/0GISsnoK6V_RXnN_0AoJ8b3RT2NZrIPts-8jUU_LhKZ_oRNsA3N8HiC5YSg-_gWaQ8MENruUuJI7cue-Jk2ZcA/EeZlYeZPs7mDEvLQhPZpmmUlyT0bzLGH8Kj5pNBM-1A\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ixNwNZP7uE1-LBVNaLVyXg/vCMq4wSSU7HeDZgz3HUVAnjU5MmhBfPWBf8dK656VgMxJN1Qu1XREIWsjevrv13AaPX_xjYcfXc3rYQ3JHZ_IQ/jDEcMRynSE1a0wgPutLvwZM9Qr7wqVnqKhe_6e7DdyI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/3AvJaZTK8d1qQz0CErSIxQ/FZC2mYfGtIJ1q8uNLSC_1TGrJo8MwLAqNNc-2r_xCufLNFvWsD_Et4PH_oIxRRxafdE8HCdM7z129Fs6oQM5fA/v6v9dzJtsTSvtlFKe4DFAABrIrG_skor4te36ie0MMA\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/CbLHYrCuNzKXM7r-Q1M4GA/6JgD_HwsfPIt81Ur5pSK1bMlUO33jFI6t_LbjXh5k8Fmd70zHoqzU1YMWv1FOD4-sBn93sN7hi1YyrlDDaqbEWCGQFrztXLaZx2EDNLLHZK4PURHljrciMgWO6ceqKylYgOGIowSPim4lBSWu-nlhw/WpNUeRDf9rHrc-FFpSmmff5Ki9XVQUcqFJmybCQJw2Y\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/2bzEqHq-E0f5WLEspPnEhA/_Z-wvZ0HqvaVQldbFks3YkXNKoBNMgycqwldlcQtQwohtlF1y4bPdbd4Fu8_-ISCmqx0VHbh3FdDJr4V79OjnndD1BG3dLhGbMcdgJi1RZtQ4nDmkTQigKVMyoNPNaOYtc9t1U1M-rrXKlGjVHCfbQ/wPg6zEiK14VTvaKICPIxnZNWPcbrSW0vQWd1hcHAQC8\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/9xz9DTK6aqnyh98v6NztPQ/xlPT5LYig876kKQvP2TAVuYZy_nM0Yzpzdbu3zNN2vqujMRLz6yxA1NTDGdNypQKsh1aVkSAcD4gdTV8_XcTFYA5RtBsVQF0mPEHp0jwPW1Q-6D0w4vi_RqvkqUx6Dx04dvhkOlN8Xm4YvzhNIaP-g/GD1QZlfG2nA2r-d9-v7vlFu90lMI-ffAILNZ7EDtwcM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Ntu5o9q2US5KGpanVF1rYg/Z30QvtPcX7XfrlnDBBnN7tHUOfDZxkYDlf-8hJPOs0N7uZP6HhRTxtnBVMNPPAtroHVk2XKbVf2a1g64naLQn3C7ZfbMkbWSybpNR1qEa6hQdfZnyql39Ed2mG5-TVvSlAQHvOIEak1_0gvme_aKSQ/wKX4p272K8izj9dbrebuGyZ6qHtZk40l5yVz9OBAU0Y\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/3DrF1L8JTjn8n-lEwKrRbA/buZfndDvL-hNJ6SP9-vBgphGzp5C0nC0rwN-696bDINxECuZOy6XNVMiOT21ck8hW9xM0ChDQ8PB37yfPnSmNRdk2LBesJgi_jrHsXt011wzg5d7pSu04cnyxJxvMBqWng-WrNMIu1wUNvjJY_Bkw2ZhyZ-mreYFbm3oS9CDVqq5naPmd1MB2Slq9dvmF0WN/sNH_VH2Tar4bwqAd3okbCIM3MLrmJIpGx-AgEanps-Q\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Qz6ISsLo1v_BOJyBn2VjTg/Fr6M5T4wL2epDJT5_SfvezS2D6zKHfKsa0AUm4iQFmaBHkufd61y_EGC7F21tKcMO9G3TA8gYe1laleDG6tcrCrfETwT6DEG0HyyBpot3GSWe3NE0t2XPyWW8J2vLpyWLJCbH737MafXneA3IfNWvLk9KDcrUMqjahDAxr1JWufOmUOfF93_NsKmiE3_BqWx/A-3SX_DlAIbfIt6WG61SOXhJDSGEEFtiE8tmUSEmK7Q\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/32ELoAAy9su1DllcsCpqYA/R2fUP-usQDDtXkmTaVvddP-lysy2zj9VDI0zTWIPE8L_GGbs9E4PQXIRgcXcTiCeZdU0SgaNMzM0ww6GLCCmI588QoMUYnZyfEkDjgLPM0z0GXBEfiA52m4JMwFj7Nqk0XLFJhKHl8pNLdG0RFdfBRt4bQAXgjLyqspyNhKYxBnh8Bsm09zM7MSam3iSu3Ar/2k6_iYIxfGku1gPOL746h0IBbqirVlCjcwxBc11-EeM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/5oWf_7KZB_msiwCqQsBemQ/2y7LuPmYQ8jeIF3Lc0kobynL334qlX37hoA-mu70OwyjSNl0MxWPZO6IK0XBzlROLZIebGf81oRKZ6veSlWFrc4z6DgfcN5FBBkrFaj6Gc5dVelOcXsSygCOJY1tlWhDdvmxncbXADom2qYBLWHWjlOYpkyKWUEPM5cV_G2K_CWYf77weEP2Mod2hn54dRVI/yBDzG1tYV3xmkFKMcNXvn4tTnEEePHdK59Y-uKA-SIQ\",\"width\":600,\"height\":600}}}]}},{\"id\":\"recaaJrI2JbRgEX5O\",\"createdTime\":\"2015-02-10T00:15:45.000Z\",\"fields\":{\"Genre\":[\"Abstract Expressionism\",\"Modern art\",\"Surrealism\"],\"Bio\":\"Edvard Munch was a Norwegian painter and printmaker whose intensely evocative treatment of psychological themes built upon some of the main tenets of late 19th-century Symbolism and greatly influenced German Expressionism in the early 20th century. One of his most well-known works is The Scream of 1893.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Edvard Munch\",\"Testing Date\":\"2012-02-01T23:36:00.000Z\",\"Collection\":[\"recwpd7MLPQqorfcj\"],\"Attachments\":[{\"id\":\"attNIEYhExe4q53lp\",\"width\":1000,\"height\":579,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/1WdEkAT2z5_JS0_h5FJiIw/dn97Usxt_4gbR0qhwbEayNnYbd1FFL8BdtYgWmZO1d1UDYg7qyC-_RlmaLqVBQE9/76T-Nu1kzqkkFWOQ1Kkj8leNyqJF-HSAUtrf7pByeMQ\",\"filename\":\"The_Sun.jpg\",\"size\":194051,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/5G1DdxWPzsHbzWdvfpwzIg/nMKCgmc5j0JaZttQChQuQnjyGxzKqFfAJQM_vzXqbxpax6iVhkRDK2jodpOAotMK/_-XEyxHtWylA3kuCFXY-eSqI3OBCS0rV---3PA1ST30\",\"width\":62,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/O5s02IhWnfY0d6lPASMSRw/kx49z73IvBs1x8HrX6b2lQKI7u07QcCpWvOpw04IeieQUAg4aej5pNBVJ4NPnRyA/ghl8qs6XdBT1zKsnpKllbgmHw4ORiQFa2PDyVmGC4pQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Fehp585nO-dCrKRhBix7xA/GoBauLy577lNdLKQ3E6H32taVMGW1rG7ydiAClLD3t18MVLgtpJrr9MezZc2t17C/FWiemyFveAoELOOqrMiHzHD69o5QOTccVbRRTmrBH4w\",\"width\":1000,\"height\":579}}},{\"id\":\"attVjzN5X8xdWoc2W\",\"width\":1458,\"height\":1500,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/grCQxB9Ud78PbS7WKDPrKA/9pd5v-ZdEDYV7nUVvphtZTLJXd0rQ8SQe4zPzr1uzRIy03NkmIi17LXFK6IswavAja9H8-vq-xKqG5jsgK91yg/TfzbPtupLc8j5X4AqTONzTMPwNMNke4RxHRGmVvjNi4\",\"filename\":\"Munch_Det_Syke_Barn_1896.jpg\",\"size\":425603,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/lxUwVgWlPplpv15Re5Apkg/4sRtr2fVCtK9JAaQviyR07852BTdFkfyCO3-NUttCXM5CFaa7WRPd9xAAoXkLmI1o9wTtfYjCJ_gecKJAXekjA/OhMOWY7ss4x6aicHXTQT-hdNi-Daz5APAs7Yq3GVU6w\",\"width\":35,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/aGNN-5i2hyuYaPlDvdlENA/Q42_DG_DOy_H_m27QlUaw327jNW_HlJhU-Rqfm-t38LHyIuy93G6AMeRl3OXjqKmvvlrTjRGDUcxMlks_1IaTA/RiwxTJPheuZPU3sTvpph225ZY4FiDq3-Gf4xC1GcPt8\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/5ftDmMiSRwzIVu9ZLBkFiA/sgcnOOS7cvFruYfVFKYmm2UxhKlI8_fjD17jF2DLF9s9R9sAOcv8500PQn4yaR5N4w0zukp6n9Qkfu2Y3ut4MA/bpXzu9hQhOe-5ZmuySbsZhEKd9x1Re6KmwOu7fS_kOE\",\"width\":1458,\"height\":1500}}},{\"id\":\"attnTOZBfCiHQhfy1\",\"width\":850,\"height\":765,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ZI8lfqsQVFJdmcj28sidlg/yR9d6HDyWjsq2Eih90iJQtHWwFzEzkWIDhUP0gO7MJeIoUTYfgsQRZMhDvhvdoo9/65OOvLP0rCEIDAeeBtHekORXnGUXjd7MsZJSh8H-Ejo\",\"filename\":\"death-in-the-sickroom.jpg\",\"size\":255101,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/B2E04Jt_OkrUDHVimE-jcg/db_E4wcNomL1XR1q3W7GjZxl-4-y2u_TRuZOWsTAagL-ibcPurZbruCxiNGZcXKf0OssLFb9AC2-K3q527kYHw/4spjv2ZG57AjhyYLAi_-icw93qAjujfW4ruGxy-HKz0\",\"width\":40,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/7HH2jUkug_GY8blfZHz_tg/NTci6mwSso44aiXgWhc9aaPhygxYgxCg4pxzElblLYH87JpwrX6BkHwTavxVDxWnzYFjKBCZVoue4wCIYCgCtg/IE8bW2ZBgghaJUKHpXbjPJJTojN89SKW9Kg-9XKCRR8\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/-Uhpmd1nZHtKM3hDZci_fQ/NVjGeAMYEy26QXI_f06411WmXnerqhZQmpUd3dWhmAMQoY2mjlNlNDqK6O_tkD0HORWK5EcOZW-2C1w-8zsVvQ/ctECfBYO_ZglWwxAVaBLGTW14gId1Xg17QBnYm3diVs\",\"width\":850,\"height\":765}}}]}},{\"id\":\"recj31Rc5TXAiVZV3\",\"createdTime\":\"2015-02-09T23:36:53.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Experimental Sculpture\"],\"Bio\":\"Isamu Noguchi (野口 勇) was a prominent Japanese American artist and landscape architect whose artistic career spanned six decades, from the 1920s onward. Known for his sculpture and public works, Noguchi also designed stage sets for various Martha Graham productions, and several mass-produced lamps and furniture pieces, some of which are still manufactured and sold.\",\"Name\":\"Isamu Noguchi\",\"Collection\":[\"reccV1ddwIspBOe4O\"],\"Attachments\":[{\"id\":\"attuoGtQSGoeWEurX\",\"width\":640,\"height\":487,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/fvGFSc5esmX3ckujnKYsig/FXMwLniIRG_BKLT6ULjnxzc7-_pgJmYpOs4ctHf7NXU/eMalDZWlQ4GCj12PvctGctaRvKQr2Y8gyCbJSmtEgSQ\",\"filename\":\"Leda.jpeg\",\"size\":55738,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/CNvm2QLk2RR-cKnBUXwONg/8CX4UxTJ-yOQNghlM48YqsCgeUTQJk5az8KELVdwvEmqrNkrQZ7yaqbxE2_vJZZ7/JCijpUwXNWbYOonRTQO-9Wxy0kfmiKvHnPcRs1oqYD8\",\"width\":47,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/G8RZLJ8c48dtoEhaJuHyvg/QW3l_V-tkIxp_2dnFH05psd8bC_GDHCQH7gYh1WfFdi7r-wTzjMzY2Bdo1b_XM6t/V2n8pErY36oHhTluMsPUaSfRcLz9WC-bDJHj12Mel5E\",\"width\":512,\"height\":487},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/w1XIxjj77YHJQQ30aK5ZIw/hDEVu4T7Suny6TuJg0EzOT9KhiN-hwgRokjM-Ia2Ql-YEtxlbkRKfu5kQsNnq0tU/TG9m0IoEyCTDWvcqZDfx-yDOCZhFabgAbvi-1VNL7Fk\",\"width\":640,\"height\":487}}},{\"id\":\"att4MPT0gu8r2DZdv\",\"width\":414,\"height\":526,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/An9RA05IiRLpj979V3sJpQ/ixiW6qg35woRK-MBEHOFZsMVrpgKeLNKl4orB1owSBjpULiAOfxaahml38fSJOco/xAlc5RVB4zIMtNZ3RrtbX9mUwAOM5_U37beOKxhdQgY\",\"filename\":\"Mother_and_child.jpg\",\"size\":38679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ryGJTmT4PuLVpuWPkYVkLg/6aF8pVm9rA7tSlm9ig_EttegrphUPX7xqDq0F1cI3CO7QnQ_umSNtDT5vD-gnVwp/nieu2ez6mgC75KwPw993d2oEAIPIPg4f4KXMzg1waD0\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/7YHio0dWXpmEIkWMZ6XL9w/4eBhffzTD21psF6SgEWD0-iEm_dMwPZrRAcNHiPal5p1WthoBanipu4PPly2iqjf/jlydV0-RW3WJX8tId0nHgXSm5GlRghJUBn0y-z4BylY\",\"width\":414,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/RJEw5GNM6EpqtRLkDzAW1A/mVRkixAHvaH95L73k6VKVZjZXAUEr5v4LZa52Z-0nU1kUZn9dPWrXcN4PU46eDYM/7zMZV4ESAkBT3R2f2FfiPU28DLfzGBtA2gwwoGsiHEE\",\"width\":414,\"height\":526}}},{\"id\":\"attsGNtljepdSppj3\",\"width\":3694,\"height\":2916,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ooF6XjITxWnaP2miWpk9XQ/Cpvcqy-hjoC4R45lxrLPu5olrfRzgdbwGJoPLvgkO8KND8Qf5YCQN07GIYWHaXot/Rkjvo4L4jmCqyFHtztdk5oqnbbrP4H3p7buA-oOhrv8\",\"filename\":\"Sky-gate.jpg\",\"size\":10002210,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/-f4a8Pv2_GcVdB1W1LNDLw/9FIppgsEyNaimR2-zovLo-jIiFneEUi6cVwkDjqmn-8KqH-a_HtjDsstX_l0mjfN/xGvGLFjJ9bXTjNaU3lpXTyLH5GPe4mhZo-Ipw637-eU\",\"width\":46,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/w51WuA-dws_54YgDEWrjXA/AIks5Dgn0yOQFkAXWC5MVW1aZdaUQcTI7INdfB3Zji4XhjHU2Rzp2pgOz1viOyZj/nWbAwih4Tj0Udj3eowZocRBf_LZf7ZbiyC0Mnqo0FBo\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/e2KzkBv6-PIOKx1XZrGl1Q/tXcSUeJ17Xj6y2GrQK9Gbw4XhFnm9NZaksCkdn1FIjljwdjEteD4SsXlVDBCw-vs/MWszvuJQ8h7e8obse2_EMORdD5Mz8bF8aD54GSQ8rLw\",\"width\":3000,\"height\":2368}}},{\"id\":\"attpYKQ4gldei2tWd\",\"width\":960,\"height\":696,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/sOC6Da1WyFqtV7MWX2TFvw/4tJpL75XESkE9FkjPhy36BpCMoyM6QDkTMn08tee6kBVRQI0Y8hdigRAvQqPhfUO/O7ZPwkk8s7ZK93JzVhbR5TLA2gj1kNGCuJ9GaXlksYs\",\"filename\":\"Akari_Lamps.jpg\",\"size\":110954,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/RFdvkbISmnaEGDLDcpIc_A/IihYIsA8II3uKOacv1fMB00Vn1HfZCh_voBoiKCTuKNBnuB-hOE8ES3jVVZekAFm/LXy0Gzo88WlaDjX1v3u_CCdL8suaATI_k6x_XXFIBSw\",\"width\":50,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/JIePB3C3ZWwegi71wCDbAA/URw3LRuOzRqCyDjUyBNYyxiZ1_X6Rqnu2kjGxjFHSPOlMTH_DRxhI0IveLgKXul3/X_hrhXUN2V-hSJ_zAe36gF80zNxJOOdxxsDOoNlBWhA\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/yxclQvM7-Qz_t572sapOdg/pYu_-pGsWd--4MyAnAr9Ag0gCPjYuPF8FKfldGNK73pr5ISVy3gRwhi8R-yyaKDM/Oi7vXzgUCuhPQ7YmKKHqlbALL4NDDApmp--PookWAko\",\"width\":960,\"height\":696}}}]}},{\"id\":\"recneNPDcZsNQDxsb\",\"createdTime\":\"2015-02-09T23:28:08.000Z\",\"fields\":{\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Thornton Willis is an American abstract painter. He has contributed to the New York School of painting since the late 1960s. Viewed as a member of the Third Generation of American Abstract Expressionists, his work is associated with Abstract Expressionism, Lyrical Abstraction, Process Art, Postminimalism, Bio-morphic Cubism (a term he coined) and Color Field painting.\",\"Name\":\"Thornton Willis\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"att8enrlgYD3FiHXB\",\"width\":433,\"height\":550,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/B9AqFBb169hiKRN5FGH4xQ/N_iatPkzLmHo4MJFn8UjXNI_QKrbiRPXT7C4ehP6PL8SPRqLdU4oNtnhSthugEsT/0UCyO2WaVzPQvSF7nbLX8ymiQLzUPXNz50sSerWbav0\",\"filename\":\"Color-Drawing.jpg\",\"size\":374784,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/dNMQzIIm8NurBNmjmSmgpA/ruPrah8ZOl3J5lDUX5HTcpjJVQ-9pvB0zHEKcCI-d3ynw-fWX5SrP03PNym5EZwL/Ysx-1A14uvIniTRSuWawwVO5TwlgJAx3UWs8Ve5t22A\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/smMm_pBlHkbJea2uVBZ9lg/aNWmgi_94sPnwfUX5Tj5qoX34nJ5ss-03xgkRL4SuTnvS6tfL4u9othXDEJTpRL4/hcTB0fub7krb6m2m6_NVwLm9vbP5zf52bT6XCj6N37Q\",\"width\":433,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Q-NEfQnLW6YjBMSuPKSGyw/THRn3RrQgqOfve8zwFlHIsvG51EhoWKOC7EWChjtfRZbJt3RWHFhTRXA3OFtXI_Q/EnDkpd8zOv8KRj2gEKXsPCH2Z3UW2XJsESnszLkPS_g\",\"width\":433,\"height\":550}}},{\"id\":\"attqEXWllptvGjPDQ\",\"width\":551,\"height\":700,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Zr1hLi5sLTtGoJROqzpN-Q/KXy_xcMe0gcxMowy9LFHXhiiJw0_HC3_LF5leIIkc_pea6s5OWsVUBDKzjDQZxED/bcTLlO7XKnIBl2WcBCRLdBM-IMEUN_mW3_4gskNcGuc\",\"filename\":\"study_No._2.jpg\",\"size\":109204,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Nd5k0Eduo-Z_N_UA8Fq_yQ/aJVcRIjl1bQK9lvJby9G_7y5bxnuqgmaI6XdZfZNq3lWqJNjA4fpO7XlOA1hYkoR/fzFivK7grsOc_tt03pNWIpFCBv38AjvB4fkb86lMXY0\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/0N8z7AvrCXNiI1iCVU81_Q/O5fHUNMLYj53d-aPWGEZLiUphnrsIneRYH5kU_eZ5paoZYAFO-FiYQacfOEH-KUG/7xPWKkL-CONSmTy9vhXDmLfVwhffs2PI7pj8Qbniu-8\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/QFbikLJPpBCfHCAExdNBJw/tzEfApmyr6BhTsCUgnT1GZOMXHsn6xYd4m4CdwWMPUGiFiC00tQAxTcBp-CP8IPa/fd1et_N6uNc6rUJlm24KUjXxedabsJcsNW9p-Lu-mA8\",\"width\":551,\"height\":700}}},{\"id\":\"attHI8lwWwEoooJ24\",\"width\":890,\"height\":1200,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/_-pjCt9ombNs2P1PbtJ0kw/J6SkmE-qlzjpOfUqbRgmsB_EDNJikD6I6VQkB7wlTFN6EPND8D8pdw8206i6pvMA/s7QcKSLcBOlGjWTfQD8dn4wQUP7Ouf7XXa-qUGyWVSw\",\"filename\":\"three_gray_squares.jpg\",\"size\":109091,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/eWUk3o4QkNUHIKoZHPH32Q/7IyqBthtDbVt93oblh9Kd7QpZIIwEZ74YbWntLToGq3hse0ReP7TKvyYv7laNmizOHyJlu1FODecjcYQmslKmw/_AMdBgCWhxwcvUAgwYf0AaoBCOit7oqa8VjcqwRDBGM\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/08rGEowY0qBl29SWuVxgwQ/7QXXEe9lYA10g8cNm_NndcRbxh9D4nSvEy3lBpAUWP41dITaY30fGqmwEWUeovKcL-VSDaQj79G-GM0cWvn-YA/loVsVh-z3KwwIwbju1J-vFazH7Fnwp6EItEs6Gnake4\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/RYedyuu_9pygC0E6fbCnPA/ZegrToyFoBIJ13ADnTSh63e1CK-tE6GTZ8iFf-WVDPFb0um5b5yA4Ngkbt9ZqvhG-npbNCTSyK3S32G2qdABsw/9OUzOy1hdn1-KrsINef-cFyVUctX4FFVmVw1zHLm-h0\",\"width\":890,\"height\":1200}}},{\"id\":\"att5TIJ00ppiNYQm3\",\"width\":639,\"height\":517,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/z98ACbOS2CJNqKn5OrKe4g/ZheLCh4R0RZ-O6V-DGz7-poVC8PcpNkohw7nXDIeJoJGPw50_UxxE0aFziQ4evAi/Tb87x3DOVNL-9NW7O5VakfimOf6S9rV5_NfIQPUNpkY\",\"filename\":\"Streets_of_Tupelo.jpg\",\"size\":27664,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/wbwjkOnc7-UTUD3A-ZXzbw/nwFBhWgDNd6ytzvGGfbHkrbrLLeNQh0_oHSrKdLaCd4Ivy_zps6xPxVDNjU9mRmY4EwMT9HkcGTNoUdYibiw6Q/dpjp1Ig1LItdal4Dnv_YcvtJaAfOi9h1pV9yqK8I2x4\",\"width\":44,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/KncA9IjVHWhN7YpYFLit8w/F2dETomdhHUOxDkwHT-SiDnUcA2kKfZVOTfDTFRWw8mmPjzOWndJAJaVIWfB4SJdeP23uTgkEycJX-KdGbOUug/g4mMuc2h2KQg3GMADOn5jKKVDcso9OBp1iI7M-LOpBE\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/-SBNlo5CvsmAQKLfr8Or7A/d0lEWlKjtxpoIpy1i2QPfB-ay3zCqLu_sTG7mKXuHfnu3Vn5hgT6yW1e4Buufuwb/-oV4j04g_0KFaAbOcORnvttePclECalIJWvsxu6LnKI\",\"width\":639,\"height\":517}}}]}},{\"id\":\"recraBPRF3m5Te5Hn\",\"createdTime\":\"2015-02-09T23:24:10.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Mark Rothko is generally identified as an Abstract Expressionist. With Jackson Pollock and Willem de Kooning, he is one of the most famous postwar American artists.\\\\\\\\\\n\",\"Name\":\"Mark Rothko\",\"Testing Date\":\"2020-07-06T17:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attONu0jXlWNlHOxh\",\"width\":213,\"height\":260,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/j0EBinTxUiJc9a56D-3Tfw/wxuKNdGzzwlPLFKIiAjtfQWkyn9YmCOIj6NRBg5-6oCbXdl5_HuUTEzMgb9t_VUO/0v0s9lc0p92dhvqmMNX66uyTg0m-CsTT_0GWXYiaoik\",\"filename\":\"No._18_1951.jpg\",\"size\":7416,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/TKFbKKiFIU21qMvWSmd0pw/-WBVrpTTCGUyvSGjQY1X3i3Awbfg8FhgviBc23WKx7-desK5d03mxa2imXRk2C5d/3oM31Xqffof_LIL3OQMNHf1PaFu63jQRApH_9Kej21w\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/IPuihQEH2oTtFBinQdTxEA/qKnAi6AdvsUhmd1BjmW8MBe1PJEUIfp_et-v0G4x3QNvOTTuUNZaO6o3qCMITL-Q/DVDvS5JArSMXxnF1N9ici--PvX5zA5BL5vvivdWxiDA\",\"width\":213,\"height\":260},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Cb__gLij2rYejxHCyGKczg/pyDsHArSDAYlnG-Z1xqH5WBJ14SJuXouEdKi2uSioDKxNTbre45m343TTWti6dr0/oESqHv6lURMnMpMp21oq6jGk7B20Hh4U52oBQJBYIMQ\",\"width\":213,\"height\":260}}},{\"id\":\"atteYo0fXP5bOpxt6\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/RRgThEw65cvHSl6LWcCGfw/8CS0zxulL8Ao0k8DPMt0DPOGBHkFXjPP4zo2fxrrENmrQB_452smll-LKEkZ3SC-/V08JMga2TYsPvY9gFt-PohfXUFwmby45VRUYDBEf36A\",\"filename\":\"Untitled_1954_RISD.jpg\",\"size\":23636,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/fOAxz_EJuPIrvq_DmpmC_w/tK8Xw0ADNoZSNnQaEm6js7w4s9mRh7Z3cKo8cC2V0A_cRms66RCM94FgcTqvV2kLmbAn1FEacUNr7nvnT8CXng/UiGNvdeB0mGUFoim6o1UlFOKnbxK6sSuamQBeyEpuJI\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/jq1c82fmz9A2JhlMbUQfJA/XN_2N0CoBfEnzTXZdM2gDr2j27P0OWBJchER4xaTXjr6iChcG8pqeUFqf7M4ERu3QxGUERl0LTYPt0GyL-4sMw/i3eGUmwJJSJwzg6v8aDyKqxJQpzOqtBXdmg35hw5Qu8\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Or9_Nsx4Br-RNqNm6SBJ2w/ozC6501wXukHZnXU9IZLTF_caQp7IH2X3P9jbpA5321bgoBHwjJbcgNA3NN1jrVy4LcIW_0wiOCTGWD2mrPezw/jzzLo2IwientFnE9DssWEJOF994pQ4qWwfMN47My0ho\",\"width\":385,\"height\":640}}},{\"id\":\"attneJn9hR5DtDns9\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/PKu-F2K9j01SuVUoQMHJHQ/0RHgbIdzPrr6v6BDc7SdbSWpducJpP9oOQJIwmUWw9lA4zOc3rTfKcIZ4asSA30u/5TsIgqdUtaMdun3a84gmvM08QrbChuiIoqXLmp84-94\",\"filename\":\"Untitled_1954.jpg\",\"size\":33352,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/rfWy1Uy98OaNnWAHn4J1eg/ze0lESdxrNHiGQKXBwuKRD1BAMheKjmHxA5tY7YVTsCUiz1hF_5z0oZ9tvPSCp7D/GTUPUsk6TdrAdwnGhOPORYsYZvfH1mKks3rTvovqedQ\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/KP28P5fUgbJwWW0HJ38y7Q/khp5mkgmDvitufJGPW5FmfTmX3ajn9LEEXF-FBXTtSOLNKd2YBbkaI_NOyxDmM6W/EpR72OftmSIaH1JRakPfvOiBDQRm5nNeVynvzpH6W5c\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/3ScPM82cdj4nyT7V4M3Zng/uZ2Y9O-2AfFMYe_U0TkvyoiHfAy5Ubp7CX8GY3_lx_krW9WeJXluifqzVsQfOxFy/dE66llE2YYYmV0Mc_XCsXYBi9cVemVnmmfXDMCrsEwU\",\"width\":385,\"height\":640}}},{\"id\":\"attnicjT3NIYNL5Le\",\"width\":551,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/aEz7F-okgVf0GAjXGPsf1A/I0uew1WRiPAlT0C6zsM2aV9ravVdhF33fy0iCQZeY_2xKKiiTUMACQIviRbh2lh1/slbOSGvtwyFAI-EcdBfm48nLjXy-IH1UrYqHHdpLCQ8\",\"filename\":\"Untitled_(Red,_Orange).jpg\",\"size\":43346,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/7IM32RAEuWSZ0QAty2CerA/0puIqwBPdeNM-FuDb21fWztkgzHtWY2XSw58i9x8hfFyTMic8U0tlSCtWRO6OMazk87ISERLSYGduql6rZov-Q/4tVT9JS6xx4pQllSnKrUWf40Br75lBq3rbyrFp4ZL8Y\",\"width\":31,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/Yh5XvCkrhQbKbmpHDXCBdQ/FNkEffELjSm-20aEV3nxfnnbTwj2dqOpwVu-x9fpR4-1T64EtoWDFTUyiq4COaL7kr3TjHCWPxGePlhPV3u1pA/M_whgyqQVkCYJlYcwcReJZjDaBfzwojd7IDbKeR24EI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/IY6p4NiaswTPO8epa9krTQ/Lu39XgK0Ga0Ag2WjOF0qCNvr_aMeeS7Zzvr33WB08j0HNHmVNih4TNNk99bL6iLutiCZwh7JeLPm4YP3zhQB7g/IATnl_4L4iEQLY6XGr3sfB5dTwWY73unaxg2dXmJ2EE\",\"width\":551,\"height\":640}}},{\"id\":\"attpDkpjaf734NjM0\",\"width\":480,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/B0fDOn85JRVX9pUm1seWEA/Sw9maCap6Hu6Z2gyncdIrZhGotc0mMKhSfDL646DrIYVvNNDraJqq40_lXCkyyeU/s8NJH9kEZQF7lcA2Qe0pK5shvcH5jI8OB7mzVSVUnsI\",\"filename\":\"No._61_(Rust_and_Blue).jpg\",\"size\":35819,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/YnIBPog9J_djh2beBBHrtQ/Yxs-Au-eXjKcaQvaNfT05LsA0sVJdOZyCVgomFHskXN4Lkie_AOMAA09fboz1kqk9bKE8q_M0cyImEOzAxQKwQ/-aUgsbDMwmbhN_jhBAOzDgOLauGMpGOMoQWtTI3bD5I\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/zVPaRquPQB-uP-yFwtECeA/Jm2MRA_tNJM8oTChTkLviqKtf-CP3xYZma3qfrf3MmZGfSKBAn1Gj14khKC2QTKGrjjcwgGKln4ICrAnD2BJ_A/5z1Jw6xlFNZtlVctvsSQWnTxm7mMOdFoBsU-vD5Mfyk\",\"width\":480,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677924000000/ZAeBmEw74_Ch1esVC8NqKQ/sgsxCn7dW2l414G9iPMI-Mt0GXEA5fuBHdrXF33fS2YGFZjMZYjtMUJgBrPR1xUPe2OVw4co8qk-t4BAumSwXQ/jI-sWwCf1gC5JhKYFTE3bSXAkPKWOy0wKTXcPXhk7LY\",\"width\":480,\"height\":600}}}]}}]}");

            string bodyText =
                "{\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            Task<ListAllRecordsTestResponse<Artist>> task = ListAllRecords<Artist>();
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count > 0);

            // Abstract all fields of the 1st record as
            // an instance of Artist.
            Artist AlHeld = response.Records[0].Fields;

            Assert.AreEqual(AlHeld.Name, "Al Held");
            Assert.IsTrue(AlHeld.OnDisplay);
            Assert.AreEqual(AlHeld.Collection.Count, 1);
            Assert.AreEqual(AlHeld.Genre.Count, 2);
            Assert.IsNotNull(AlHeld.Bio);
            Assert.AreEqual(AlHeld.Attachments.Count, 4);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TcAtApiClientFieldsTest
        // List records
        // Use the 'fields' parameter to specify that only data for fields whose names are in this list will be included in the records. 
        // If you don't need every field, you can use this parameter to reduce the amount of data transferred.

        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TcAtApiClientFieldsTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"Name\":\"Al Held\",\"Collection\":[\"recuV4lqy2awmYEVq\"]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"]}},{\"id\":\"recaaJrI2JbRgEX5O\",\"createdTime\":\"2015-02-10T00:15:45.000Z\",\"fields\":{\"Name\":\"Edvard Munch\",\"Collection\":[\"recwpd7MLPQqorfcj\"]}},{\"id\":\"recj31Rc5TXAiVZV3\",\"createdTime\":\"2015-02-09T23:36:53.000Z\",\"fields\":{\"Name\":\"Isamu Noguchi\",\"Collection\":[\"reccV1ddwIspBOe4O\"]}},{\"id\":\"recneNPDcZsNQDxsb\",\"createdTime\":\"2015-02-09T23:28:08.000Z\",\"fields\":{\"Name\":\"Thornton Willis\",\"Collection\":[\"recuV4lqy2awmYEVq\"]}},{\"id\":\"recraBPRF3m5Te5Hn\",\"createdTime\":\"2015-02-09T23:24:10.000Z\",\"fields\":{\"Name\":\"Mark Rothko\",\"Collection\":[\"recuV4lqy2awmYEVq\"]}}]}");

            string bodyText = "{\"fields\":[\"Name\",\"Collection\"],\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/listRecords",
                    HttpMethod.Post,
                    fakeResponse,
                    bodyText);

            // Only data for fields whose names are in this list will be included in the records. 
            //If you don't need every field, you can use the 'fields' parameter to reduce the amount of data transferred.
            string[] fields = new string[] { "Name", "Collection"};

            Task<ListAllRecordsTestResponse> task = ListAllRecords(fields: fields);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count > 0);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TdAtApiClientFieldsTestUsingFieldId
        // List records
        // Use the 'fields' parameter to specify that only data for fields whose ID's are in this list will be included in the records. 
        // If you don't need every field, you can use this parameter to reduce the amount of data transferred.
        // However, using field IDs, instead of fields Names, will still have field names in the result records.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TdAtApiClientFieldsTestUsingFieldId()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Name\":\"Al Held\"}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Name\":\"Arshile Gorky\"}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Name\":\"Miya Ando\"}},{\"id\":\"recaaJrI2JbRgEX5O\",\"createdTime\":\"2015-02-10T00:15:45.000Z\",\"fields\":{\"Genre\":[\"Abstract Expressionism\",\"Modern art\",\"Surrealism\"],\"Name\":\"Edvard Munch\"}},{\"id\":\"recj31Rc5TXAiVZV3\",\"createdTime\":\"2015-02-09T23:36:53.000Z\",\"fields\":{\"Genre\":[\"Experimental Sculpture\"],\"Name\":\"Isamu Noguchi\"}},{\"id\":\"recneNPDcZsNQDxsb\",\"createdTime\":\"2015-02-09T23:28:08.000Z\",\"fields\":{\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Name\":\"Thornton Willis\"}},{\"id\":\"recraBPRF3m5Te5Hn\",\"createdTime\":\"2015-02-09T23:24:10.000Z\",\"fields\":{\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Name\":\"Mark Rothko\"}}]}");
                
           string bodyText = "{\"fields\":[\"fldSAUw6qVy9NzXzF\",\"fldE0muAk6ejOkkKa\"],\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/listRecords",
                    HttpMethod.Post,
                    fakeResponse,
                    bodyText);

            string[] fields = new string[2];
            fields[0] = AL_HELD_NAME_FIELD_ID;              // Field ID of the the field 'Name' 
            fields[1] = AL_HELD_COLLECTION_FIELD_ID;        // Field ID of the field 'Collection'

            Task<ListAllRecordsTestResponse> task = ListAllRecords(fields: fields);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count > 0);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TefAtApiClientFilterByFormulaTest
        // List records
        // Use the 'filterByFormula' parameter to specify a formula used to filter records. 
        // the formula will be evaluated for each record, and if the result is not 0, false, "", NaN, [], 
        // or #Error! the record will be included in the response.
        // If combined with view, only records in that view which satisfy the formula will be returned.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TefAtApiClientFilterByFormulaTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/39oBWza7hOYJJgaVt_yRpw/gaJUvLehOOfXILepGr7JiefPNcFMCedzVWknVqxiiRvHI9zqiWWbDW2OeIy7bnVj/OTmb3wZFcrc_MX-93zh_amU8azw1ROc9Mi-AUFvoWQ0\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/f4CQhJC03yufCMOak31dbQ/vwF_STC9EgXFsUkzCY-KG17kQnz8a-iDmBuKaaD3n-8pzXBJAvtDpnczqkuGtVTpYZcV3lYGLY3iX4qaiXnJqA/RlDEyWen99f4JeSuYurxTSFKZNlDd4v5tjYSoSpG2vQ\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/g48Aovs5yGIf2ZgLZwH2BQ/eyoJRrpw3IE2DxYX-_RP9DerRvEnFhPVU-Snqb684bwRb6qmRCguRRcBmszs9Es60AcrRCj8d9vR3ps7_xRSPA/EG_wDl7vmfQdm91Lh_N7IM0B87G-nLgkcsuALExevps\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/20mlEBWc6Bovof2ASIX1Zg/coJPJ4VPg24Q21fcIBI7SK1c9KZYW3zhSBDNORjrGgmDC_smPFerLHOVZOcBOUCPIizTpO9RRgdjf0VqUuJAXw/syA1G5xMrsCLEyR4VHnGss7n9sIqx-gjWPSOco5QFHs\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/I3-tYuTmDu89tn8_683myg/VnVWv9cniMd-24LQzZbNCbv8Y8LEdNcx0ngI6eHvH-yTE-zUJ8OWCBARH3-mtjJL/e-vhSIglR9avANBAL2rg-uOblPywKQ84TRVDNwiEGT0\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1Zxxi0BDpYmsBKka6hWATQ/C7nmfnT2nHHiFxQZEGa48v7bYDWdN0ISp5M2YT7klzBT3UvUjhGvpgvDRKBBRtm6/DHlRiTDu3Y8XtcAB0tUdvM_ROp-5W-yYNmGBxpEj7SE\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/TkpiadKNPKchJFC0nozMUw/7D0P5PjcQiHxt-R2gcV4ECwDwePR5t_3pQ-3V8Ieb0bbQEAkeKE4eHXqoawmqk7p/0Nj5NKoqhJtCHGiKI6Bapn_mUzI19LsLZnbKXEgDXz4\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/mAcRerRZUjZgNN0MMIZK7w/Drvhmq1inhnqUJaTTxiOhedWpjRHksluhv88-GXNm22BO37fLmbQszVSyyCL1syw/zNh7DmoMdPdDON8BgfQRSfabQf2XrOrjF1IRzn2xE4I\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/LJLILvRGC_OOu1kzFeclfA/8o3klF3vxIzq1MBJH2DH8RGvQTTpj9OGTdClHm78bgh4T7Xr_MbFAq0EGwo4k9sh/5j_h5ZCEmwPMSvEcdXRSLOHMaMdIM2DkpF13sfOAVog\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/tKwV1-9jqI1NfAlzp1mC6A/Kdnz2ID7WL5LaxKvLtY5mUG8NWZ-ypzsroTjiMvi88biBOPrCgfoVLBRGjsi0UiJ/AKXvKP63YcFBV0v5d5Lszmx7So5Nnw0SxpAFN1QYe74\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/P_XjuIaj3FZPiDALCLc_0A/6C4ktS8WKbnlSA1pmxKJV5yJS_AjoVAqhgSz2u7kSduwFGgY6B41WFoOgw58DaH0/36wu4pZXdqujq3GBu9tHwzQ4v0lqSnMry9hibfmWEnQ\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UrKyE50u0_m3DXznTljomQ/IqZyGe0im_rdzvvw8r4RkeiClqKrSARXR3CrebsI5lo4RnQ9HiN6hJyaFYM6J6lU/eTYwF5IhXhqqqdORJVEUKvObU-g3OIGWB_FbYXP7it4\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/AFMISdEguC43D-e2t6BRhw/id18VPoJ9MoFmTl-cwVkq7F9ifIfstM_wwhkkTaT4Ce0OSa2blTBzj1Ubcco0CeL/GJ4aC28m1h-QBvdhPtjZpr9kM_2AqxtvliU0ehwFiF8\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/r_vFrkyQ64TZJHLTmPAmlQ/VSDxC6OwSu38B_p40dStq5CaLkPZgdKmD7rMPHbco9e8ErnVIWe767wNT8hHkThl/YomqoVD18mfLgT3E8rCVHy64gaXWBkZxJzeJeGFKuA4\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/fEaI7AaMNNosIfAR3MI3Zw/NIS5mLHKpmD-3NAVhQhEZH8uRV6CT43bn4ndngoc5r63X8FNcE1AQ_YqFyKvcA17/98TzCdD66Ug2lnWLmjQupsqJFWlrfMJo9nyAIJ3FUOU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RVdlXPRU-sF92yAU8zHM6A/vkpdS8zx_NvRBSDlxSNxu5-nBiSmKbz3r0hj9-nGXybutuAeFfm0oY6x188TW6EQ/s8e1sWCvynjZ9x9MXBe5mnQUfWaBRDdMkVGko6yywoQ\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/dyGthKoDMnKwSI2KTA6wVA/UMVA2dQ5RSnfjfNlvN2--fzHihEvEsSBIjd9e5DH9TmNCyrrPgnyibB_aRuxpQnw/7g23WoYII1RjnZpHinLSkSfw-Dx_2zHuCTjpYiFm8ic\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RhEEH9XTSra1BtNxqAnztg/i7j3E9P8ZZs_CIJWr4ZHS6JHHgL--aPY9oiuf2Vee7BON7AQ4N-YXpXpbEIo4rxe/47rBsXKdVYGcTTgVnu3FXtnmZ0DyF8OXnpzmvPIsAfI\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/df87qbzGy6wJIanNCyBtrg/xUy28IShaE4kuXQ1qv9VT_xi8NMLRJNHGnowu7-axpnXr8mdoKjAFsLAvSRfL3UU/QqaimoVqaY-uqYPEac5fzhpkDZfOFZ6RZg0qIEderAA\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/rVIShNrp_34ji6XuooCMeQ/NnyruykjDN49ZGydPKfhe3CCVFdvyXZFYdXG9pqwGHET4AOGuKO_zYveGxZHzpz3/WEw11im0irKutvWzzuY1QroU95RzCr4RuwyksPha854\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/5oVv6rSzFGnN19MzVCL7pg/6_qT7KCq4jztukRdwBG17FAZFWBqkb_0pGwAC1nsn3bORCFwNXYjrWTzrLVP7_RQHwj98_FOgIyv414dylnmdQ/4F1nzs-VWwB5nW_zYG8eCGjw-JXuNWCqnp9fo_WozfU\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/nVkcUIfjWGvko5xzvtB_Gw/Vl_T7OaOV36M267RTg3p_ymwc5FMzj6vySE4MXkDtkRJjTWnkDjVvrleKNoQIJ7YH6Lhswqvp0LK8s8TW0CAug/hWdhfxnxEJ-JpWXibAArUE3ZoKHaiSas2XI-J-YNRz4\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/J9LtV7PbhIe_l-mgC8whVw/kwSbJnyHqWwdWc7GY712d3SgS-BKiPOh9ChBYCd_XXZaaUMc44991xsS487QW84W4E8zZMx9hmJuzVPnYrWywg/uhm_Q7EEB6UxIE-aRVp0yaRRgX5b3Qswfv0t8JZBjGo\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/8gY54C14V6hkctOxYFi9jw/rDl82T8e8XRjQGdV46JM5oBCFLpzKYDB4I4YQhbRilLPsPtRfvo-rkuHmOvz5VoJ2Rksj1JSur4Xr94nCc6_bw/oYGJpcqKaG8XZmD5ictdRsVr-tKixhBajfDgTjdMwzI\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HupXpyJP0RMjjfULl3JRrQ/eZg5CfzE-d7EBZmRUDIhv6t3-bfqWWllXlYeufGfTxpwTH44c5dD-hZGvKYBHgNA/AxWJsigWpfxcjPtgHkAP-KVO1SkOAjP1YiI81iTRF3A\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NvQxlM9X5NQW7zOp4AOkTw/jW08CtxJQoA_rq2EvQOstuFPe-XpvikDGRhnjCFow61fqcYFft3CDz1Z2PR1BPgcNGfUqLmTjg-Yf7ZWXUtRYQ/H-sIC8zzV7Bxdf7LwYKc47zRL7n4gcAyncbygJskoAY\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1WwjreUt4BcwqqGu1_jlMQ/t3kmPFVWdlgI0zO4cyzm_5hnhAcfzJT-2p6LOHWZLd14shFRhoAfuZIAb3_UCo5ZQyrxCmHFFdkIzOrgK_Hw-w/I5C5EcuW1o2qzBGBQAuseP9XerIshRbFpsFn1xG-qHs\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Zg0G5_M_jwKtmw11dPPCHg/HEsRVXchLl8s0X9WtwmUAELkhTOL2mYbHTuCsqlwHpvSggYzqG--je8Kaqix-rH4D8uOzhFIYrCJbdOlCd349w/oaKrXWTLApH2eu4Spay1lOvIcfUCMgyeBrTs_xozCxw\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recj31Rc5TXAiVZV3\",\"createdTime\":\"2015-02-09T23:36:53.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Experimental Sculpture\"],\"Bio\":\"Isamu Noguchi (野口 勇) was a prominent Japanese American artist and landscape architect whose artistic career spanned six decades, from the 1920s onward. Known for his sculpture and public works, Noguchi also designed stage sets for various Martha Graham productions, and several mass-produced lamps and furniture pieces, some of which are still manufactured and sold.\",\"Name\":\"Isamu Noguchi\",\"Collection\":[\"reccV1ddwIspBOe4O\"],\"Attachments\":[{\"id\":\"attuoGtQSGoeWEurX\",\"width\":640,\"height\":487,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/yZTgKAaWcwfCExKnoEB1tQ/HoJrgwi6NUTDRp2qLH770PeeL3XIa4I5USmX9WR9hWc/rvhVZFT7v0RIwBMqsn2rvmpqh-4SUmcmoPbFkjXJreM\",\"filename\":\"Leda.jpeg\",\"size\":55738,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Xm984HlTOt-Evm7XypPheQ/M_yOhO_xPhMQ7vqJeeocbIBuy6hQqaTDSgD2Ttf9Ajbnc-mlAR9ESq1S0YMHYua5/lN29NGXCkjvoWJ6rTPtLXS7ujrqZW7oYtJOXhZCE6tE\",\"width\":47,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/TooUBGJwwGtHiAloT48u2g/jmA5DW0O9er_mBrNlus0Ehly2PZk5Zpd-7sp5b9GyfZCLaD-udl7ZJTCY-hwLAjm/TreykLTJP_5xSE_OH_sa8uwX9V0wOj8fQNt_77lMj9A\",\"width\":512,\"height\":487},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NWGeILM__eyRrhsYgG3KQg/9AP9mOkSq1zGiJOXvZSV_hvdVgoSFKIKbx3KPLcDolxqIHie8y3W2eTl6tLE9gPw/xSyZztnKIrLt82M1DGH6r-V7RpbFnVjTgn715Fit9r8\",\"width\":640,\"height\":487}}},{\"id\":\"att4MPT0gu8r2DZdv\",\"width\":414,\"height\":526,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/CPUzMG77y3K0rA0tWU8o6w/pxh4dReWiftrJv9b1Eo86OOF7-_3RJoVp_L52AwuXAnN-YKeePsqpSRIIu7nTcXm/6NHW8i06htQyw_dGyllAg_JBesrIQB0wXcKaN8VAU7g\",\"filename\":\"Mother_and_child.jpg\",\"size\":38679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/5SWhqoMS2WsDHfm2Vobo2Q/pARCyYR-nkEhGsLYPv5lMXj9AKzmgA4aG6U0CB7oetcTy3WW4lRO086gVYClRm4d/i6tG378kTD402KuJc3OrhtBZbGXPo0X1Qup4v3HVRDA\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1o61CS4NAY95mVVFZkFAHQ/87ublhCOD7O_vw9_CorBWGLDhaIDfTIePp1huUxhSKyPRn4d3BhkOoChOE7I7Vc2/o2NLtrXqBvCdfyopdGDCv0HAVK6yIjVkuA3F8kfFKJA\",\"width\":414,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/3w03xXXAp-vAHrdQ1i18pw/DFYQN7JVMKn9VMS0MR5_LGHmTuUB104JN53svlkhuUNWrWwAPtvv7eE5aupLmBWa/IEHegAdxVxSkrL_FfHRfji4Vc4FcFxC6KuWY-E161Vg\",\"width\":414,\"height\":526}}},{\"id\":\"attsGNtljepdSppj3\",\"width\":3694,\"height\":2916,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/f8iRSVC3GILZXqUtQFrq4Q/cUdshOQNKZFUiYLfztB1F5YPwpBNdr5P2gChBUycvuT1DvfQXF5YFQBj3qykjTud/Ane0w5HrvGVD-6vmiygh3BNs6T9ovEsgO7eUQSc3f5A\",\"filename\":\"Sky-gate.jpg\",\"size\":10002210,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/QwEQlQixgOqkn8nslN2Y6g/IL83Rg0u9KpO679by57L4N5B9_V3usQXnSKHoZFRWQyr5RuepZIdHn09MdbDoRjf/JHL13nwJckwS4Zd7bVQiVpao6iVZpfsKXeaIKApWLWA\",\"width\":46,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/oKxweEQcMivtBwWJbokbPg/uc1tD7j9mfSvU2ZHf02ypEn129Auez_gJIompbI3vApbaeCX-NfjF82SP-JKhnFR/2sjkt0hOM6jzz-mOMVXllnlnQKDolzpLLn406-CTQJI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/kFallbj8w8BCSI_XC1Fqwg/TGbmFkXXU04v2xR6IPxu0KUOOrvdeLDtsXaAwa0iX7xoVbl7xKfqdx1MoWWC1ZSt/NxNxeFf-LUSSvdI5GywlF8IrKrtYDSgvYnPq1pn2Jyw\",\"width\":3000,\"height\":2368}}},{\"id\":\"attpYKQ4gldei2tWd\",\"width\":960,\"height\":696,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/SOSc7Wvam2A2Poh884R0KQ/QEvi6TyQ365GuWaR-vvCddbyE2SixWKu42TpQtm8pQN8CdEkJXnMwaMihOHT7Ria/PPJY4tf-bw7aByqzr3nxouFe7rjoEeMS-nPvTbFEJAU\",\"filename\":\"Akari_Lamps.jpg\",\"size\":110954,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/vdQhCL3Sf0o0WvqdfcvZGg/tMEhb2vxY-U_9RF7yukhGgVJhK50xvUn8VkCe0CNgEDDli4adUiZTa4fsXMK5FvJ/JAID86P8QZqVDza0tOLsOW4vNEw-wDLBygMefJWHcS8\",\"width\":50,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/YOypgJVIPhF3Ad6kRmyi1Q/tuaUGFPR7ibfhl6Ig8bkXxCmNhJvUIuW0ET7uvFYWFZteb97O67eWvj1DsQ8miea/6BptX9BHxZnpyyVm2AOrsW9UUiBGS7bmM01QPt09HzY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/vaqG64yXJeKq4nADSnVVLQ/1s8MWpB87kemh4xvb_b_hkMU3zTFCdIGgQSlswo8uLMBNkaljnGH-Z-CxOCdeM1O/Stgp6l-t3I4TjU1ZPfJbKJkDr_kJEwXPP3qJnLFvjmY\",\"width\":960,\"height\":696}}}]}},{\"id\":\"recraBPRF3m5Te5Hn\",\"createdTime\":\"2015-02-09T23:24:10.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Mark Rothko is generally identified as an Abstract Expressionist. With Jackson Pollock and Willem de Kooning, he is one of the most famous postwar American artists.\\\\\\\\\\n\",\"Name\":\"Mark Rothko\",\"Testing Date\":\"2020-07-06T17:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attONu0jXlWNlHOxh\",\"width\":213,\"height\":260,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/PbUV3GShksF7eS90_rrBJQ/jM8_jTwippopEtSruqAyKnanitl7tSTSyYPLJox7LnBkgdRMVAYbGbuNfk4cv2UD/GIugB_QeOYiKQlZhbceOh5Cy2UcO2sfDCT8hJO-CM68\",\"filename\":\"No._18_1951.jpg\",\"size\":7416,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/dnTWghS_CLbLPErUdpkYlw/FcQvNKRB0IUQfS_OUKiPhZiMK36nI5uh2gw-MRxAH0SiW4bnY4YjHa9dc_3F2hm8/yu8xDg3MnpBP47geWMbMApOBamBdu4GMLNisTHSw5KU\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Owm3s_T0ClZATmWYh8Yd3Q/KNsh-aNVWjL63-eAeeiKCjCNthx0OboZqnzNZTeZlIR2rG4djaZuoYI2rchAuU6i/gWGUHp15-bh2QVVTe3fU7-4pFv_69DbvA-RZnTypZ80\",\"width\":213,\"height\":260},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/QFT0oniW5fRt4uerd76_UA/dGGo5QWu_PJH0lmYBvOp3YLMuGZprv6uRwJckR64FB3F5sph6g-fpdJDWUWRJN_x/yuce_4yYLPycwODVjZjJrPoz1x7y1ERdwS4vVgmDyX4\",\"width\":213,\"height\":260}}},{\"id\":\"atteYo0fXP5bOpxt6\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/jy6-f9zfGfaQ6GfXh46Eag/8hAPj2fzKgkSkBaTc0tgHPY78yf9uf3KOOX-vimCysVIGEcyZisxIWl0BJlrL0Mi/40UHv833uyx9BnIMeBQbWQm8SHHKZVegG8kBp_OEph8\",\"filename\":\"Untitled_1954_RISD.jpg\",\"size\":23636,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/BfHoD3VhEG2sfKySKfYGpA/j1ihCP8vlaM-YLo4AYJROZrnm9uf6_IW7LAEWOjIBLzw_X7EGWbxDrsZEM6FrupX_YoCc1EXHEtBXr5l6jLh3Q/eQF8yaXoqMTV94o2to6uKCDYAYc0jglFKbMe-t05_sA\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/kM4zjv9Kg2rQ9R2uIKsBVA/vXY_-TIXn2v_8rc0EpPnJobnNejdiDyubBNa2a8FXcrMTChgY9r4IdyMqs--NkBrsVg5eKAgRR4MqCfHRti0kQ/FDuAxDUXBFjTuIFdIWx7bo21iBfmR6lZsZwSnxg90d0\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/tpxRiskTPIUHcFwamGG2lw/YhzPHu_kgmadbBr16ZSmvtnMainogo-e6qjRdyv-yQQ7if6hb5Qo44r0Cqhftwo9puhko96HE03tA3GMzOcHPA/GgkK4JDHGox8xsjx1tfyFM9_KO3yp8UAQGH7j1t1XJY\",\"width\":385,\"height\":640}}},{\"id\":\"attneJn9hR5DtDns9\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/ek8PNYj99sbtEYG546HJjQ/M4lTD-70Sp7ZW2ySxFOIr6mHG6QlZZK7476OZ900ABMW8YudhRz77ELOnt5jxS5i/0TdF7mfi6b1lTBRfLqHZtYbb7nEceEjsS3QmcYqEsiE\",\"filename\":\"Untitled_1954.jpg\",\"size\":33352,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/ksoZ22Zhvxj_GqWuij14Xg/dD-UyXwVh2gmBZlmX3fmHSCg7pFYBVsuxUi5KA_d4rmPKpab8NBdLsh8kn9FCmcQ/J05MFpNIYBRnsBksyWTr0yyk3LgMg9VhniAmRVfHUY0\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/QP6YIS7IA4qxDllTCBJJXA/IV_nNgQjbI8-5kNPTnNCbq4JZZqsCh2YLnmiXFK81W_Csqn48n1nZNLScRKAbMDy/JtvnBui_46sAsKMJcl06QIk3ChiEhZvjSwY3UaDKbGo\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/70hsT0xUHiHo1TqyspxZAA/FVvlgrYmT3-TL8LxkYVhAOH5EqNjgwLaD3R7NYOb6IW0ijZyl-MqxBFEdkExS1TW/TnwzNpwwlH-0phY_bJbVsNqCV1MTN_USzHtM8044ef0\",\"width\":385,\"height\":640}}},{\"id\":\"attnicjT3NIYNL5Le\",\"width\":551,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RCSRZQDTEcw0hSma0CTf9Q/5J0xZocW3v2EJmAqFMysSTDwAgwLd4pGLmuFlhb2VyQy3v2YxdCj2bl8IoT5iNEq/aa7M3e4EXDJhVSMyouhlY9BfTy4zV_T6P1WhBXsrExw\",\"filename\":\"Untitled_(Red,_Orange).jpg\",\"size\":43346,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/MRK_9Zfb1-QoyAW7_fYjag/z5o01IpdLlRHExEp0-BV_YWiswTqh75ql2Y9PC98c-LIepXOVaEEWyU2MbkZxaEKkac62ZhQoHpuynIxAlvTig/OkE3ZIBBKAvMMtOqai_LEbXoHfPgvpMHjUBT6d2qhnE\",\"width\":31,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Nrk9zEjCb--X3e-CaDzHWQ/yVIFV-bnqEeAoiFTd8KC4UQdQxEBch7NUaRo5MrbxTSRoGtE99jlayyntii4qrjLjUMpmOFjGlTMBIuRxIF6wg/KhyOEj1x94gBLsswCwdfdtV80ylMdKtVKKcXODlO2TU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/KR_OHMXJgb2LTPg2FE0nqA/ZMjgjzDe6d1k5TTtkx1vzIk8QkLqriKh8l0uHYVGcc3vRcWUrynaJdVE0IM4PMf0UHoF52ImAUiE0DbxYs46CQ/Ms8AceaI5PADU3NdQ76qKZ7FROBEOHNDFNFVUKEKAxk\",\"width\":551,\"height\":640}}},{\"id\":\"attpDkpjaf734NjM0\",\"width\":480,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/4EliRVvEbjBXI_HG8UKJYg/2HOaXvOKkXb-VlV7xjDFmvb1ecqnEowy2TjwgBV37Vnr-hkVV-cl1-VuUfENaZFn/Oq5LoXCt_Lew6HzFqpzpxybNXiyDlLTicbu_qJudK9U\",\"filename\":\"No._61_(Rust_and_Blue).jpg\",\"size\":35819,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/F4W1skt82SPkKT6TYTj8fA/4T_w88TdVQoURMCX4kIwrMiwSOPgziQ2d_x53iJMJzV_xw6cHHagvHACZ0cn4C5hmupL0b8TXcV6CGt8BhS8IA/z_DKt7vrWDAG3Dc7pQxRtm2ZMaX-frUt-ne8J8AzPDc\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qhM_BCvbZIOd9QxppXnevw/0G_3GP_unGeDXRX4zu5kUfaMRYsyYMjBeeGg0WDxn9Tg1xRiieH9mlgdwcm5oT1Cxp4tXgx9RF0dCBF4Eky61A/4R7uYoLEKjUxZyUVvsBAZGTIf_oiUn2xEcDwL47Org0\",\"width\":480,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/aczP_KWUlOjTx4oWXPFxRw/CgTAwAwDUtklKVLb2Qb69IvgWN_vI1ZrVUUMCpVkGogpAwymvOKWT8uR9QMD84XeajX9AoEKNroA5I4PIgWdfA/xNOe9bfOoAimon4Y579IQf8C64Qa_SIMWjCBFVKzw2k\",\"width\":480,\"height\":600}}}]}}]}");

            string bodyText = "{\"filterByFormula\":\"({On Display?} = 1)\",\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            // Set formula for checkbox "On Display?"
            string formula = "({On Display?} = 1)";

            Task<ListAllRecordsTestResponse> task = ListAllRecords(filterByFormula: formula);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count > 0);
            foreach(var record in response.Records)
            {
                Assert.IsTrue(record.GetField<bool>("On Display?"));
            }
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TfAtApiClientMaxRecordsTest
        // List records
        // Use the 'maxRecords' parameter to limit the maximum total number of records that will be returned
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TfAtApiClientMaxRecordsTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/39oBWza7hOYJJgaVt_yRpw/gaJUvLehOOfXILepGr7JiefPNcFMCedzVWknVqxiiRvHI9zqiWWbDW2OeIy7bnVj/OTmb3wZFcrc_MX-93zh_amU8azw1ROc9Mi-AUFvoWQ0\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/f4CQhJC03yufCMOak31dbQ/vwF_STC9EgXFsUkzCY-KG17kQnz8a-iDmBuKaaD3n-8pzXBJAvtDpnczqkuGtVTpYZcV3lYGLY3iX4qaiXnJqA/RlDEyWen99f4JeSuYurxTSFKZNlDd4v5tjYSoSpG2vQ\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/g48Aovs5yGIf2ZgLZwH2BQ/eyoJRrpw3IE2DxYX-_RP9DerRvEnFhPVU-Snqb684bwRb6qmRCguRRcBmszs9Es60AcrRCj8d9vR3ps7_xRSPA/EG_wDl7vmfQdm91Lh_N7IM0B87G-nLgkcsuALExevps\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/20mlEBWc6Bovof2ASIX1Zg/coJPJ4VPg24Q21fcIBI7SK1c9KZYW3zhSBDNORjrGgmDC_smPFerLHOVZOcBOUCPIizTpO9RRgdjf0VqUuJAXw/syA1G5xMrsCLEyR4VHnGss7n9sIqx-gjWPSOco5QFHs\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/I3-tYuTmDu89tn8_683myg/VnVWv9cniMd-24LQzZbNCbv8Y8LEdNcx0ngI6eHvH-yTE-zUJ8OWCBARH3-mtjJL/e-vhSIglR9avANBAL2rg-uOblPywKQ84TRVDNwiEGT0\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1Zxxi0BDpYmsBKka6hWATQ/C7nmfnT2nHHiFxQZEGa48v7bYDWdN0ISp5M2YT7klzBT3UvUjhGvpgvDRKBBRtm6/DHlRiTDu3Y8XtcAB0tUdvM_ROp-5W-yYNmGBxpEj7SE\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/TkpiadKNPKchJFC0nozMUw/7D0P5PjcQiHxt-R2gcV4ECwDwePR5t_3pQ-3V8Ieb0bbQEAkeKE4eHXqoawmqk7p/0Nj5NKoqhJtCHGiKI6Bapn_mUzI19LsLZnbKXEgDXz4\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/mAcRerRZUjZgNN0MMIZK7w/Drvhmq1inhnqUJaTTxiOhedWpjRHksluhv88-GXNm22BO37fLmbQszVSyyCL1syw/zNh7DmoMdPdDON8BgfQRSfabQf2XrOrjF1IRzn2xE4I\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/LJLILvRGC_OOu1kzFeclfA/8o3klF3vxIzq1MBJH2DH8RGvQTTpj9OGTdClHm78bgh4T7Xr_MbFAq0EGwo4k9sh/5j_h5ZCEmwPMSvEcdXRSLOHMaMdIM2DkpF13sfOAVog\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/tKwV1-9jqI1NfAlzp1mC6A/Kdnz2ID7WL5LaxKvLtY5mUG8NWZ-ypzsroTjiMvi88biBOPrCgfoVLBRGjsi0UiJ/AKXvKP63YcFBV0v5d5Lszmx7So5Nnw0SxpAFN1QYe74\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/P_XjuIaj3FZPiDALCLc_0A/6C4ktS8WKbnlSA1pmxKJV5yJS_AjoVAqhgSz2u7kSduwFGgY6B41WFoOgw58DaH0/36wu4pZXdqujq3GBu9tHwzQ4v0lqSnMry9hibfmWEnQ\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UrKyE50u0_m3DXznTljomQ/IqZyGe0im_rdzvvw8r4RkeiClqKrSARXR3CrebsI5lo4RnQ9HiN6hJyaFYM6J6lU/eTYwF5IhXhqqqdORJVEUKvObU-g3OIGWB_FbYXP7it4\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/AFMISdEguC43D-e2t6BRhw/id18VPoJ9MoFmTl-cwVkq7F9ifIfstM_wwhkkTaT4Ce0OSa2blTBzj1Ubcco0CeL/GJ4aC28m1h-QBvdhPtjZpr9kM_2AqxtvliU0ehwFiF8\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/r_vFrkyQ64TZJHLTmPAmlQ/VSDxC6OwSu38B_p40dStq5CaLkPZgdKmD7rMPHbco9e8ErnVIWe767wNT8hHkThl/YomqoVD18mfLgT3E8rCVHy64gaXWBkZxJzeJeGFKuA4\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/fEaI7AaMNNosIfAR3MI3Zw/NIS5mLHKpmD-3NAVhQhEZH8uRV6CT43bn4ndngoc5r63X8FNcE1AQ_YqFyKvcA17/98TzCdD66Ug2lnWLmjQupsqJFWlrfMJo9nyAIJ3FUOU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RVdlXPRU-sF92yAU8zHM6A/vkpdS8zx_NvRBSDlxSNxu5-nBiSmKbz3r0hj9-nGXybutuAeFfm0oY6x188TW6EQ/s8e1sWCvynjZ9x9MXBe5mnQUfWaBRDdMkVGko6yywoQ\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/dyGthKoDMnKwSI2KTA6wVA/UMVA2dQ5RSnfjfNlvN2--fzHihEvEsSBIjd9e5DH9TmNCyrrPgnyibB_aRuxpQnw/7g23WoYII1RjnZpHinLSkSfw-Dx_2zHuCTjpYiFm8ic\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RhEEH9XTSra1BtNxqAnztg/i7j3E9P8ZZs_CIJWr4ZHS6JHHgL--aPY9oiuf2Vee7BON7AQ4N-YXpXpbEIo4rxe/47rBsXKdVYGcTTgVnu3FXtnmZ0DyF8OXnpzmvPIsAfI\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/df87qbzGy6wJIanNCyBtrg/xUy28IShaE4kuXQ1qv9VT_xi8NMLRJNHGnowu7-axpnXr8mdoKjAFsLAvSRfL3UU/QqaimoVqaY-uqYPEac5fzhpkDZfOFZ6RZg0qIEderAA\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/rVIShNrp_34ji6XuooCMeQ/NnyruykjDN49ZGydPKfhe3CCVFdvyXZFYdXG9pqwGHET4AOGuKO_zYveGxZHzpz3/WEw11im0irKutvWzzuY1QroU95RzCr4RuwyksPha854\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/5oVv6rSzFGnN19MzVCL7pg/6_qT7KCq4jztukRdwBG17FAZFWBqkb_0pGwAC1nsn3bORCFwNXYjrWTzrLVP7_RQHwj98_FOgIyv414dylnmdQ/4F1nzs-VWwB5nW_zYG8eCGjw-JXuNWCqnp9fo_WozfU\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/nVkcUIfjWGvko5xzvtB_Gw/Vl_T7OaOV36M267RTg3p_ymwc5FMzj6vySE4MXkDtkRJjTWnkDjVvrleKNoQIJ7YH6Lhswqvp0LK8s8TW0CAug/hWdhfxnxEJ-JpWXibAArUE3ZoKHaiSas2XI-J-YNRz4\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/J9LtV7PbhIe_l-mgC8whVw/kwSbJnyHqWwdWc7GY712d3SgS-BKiPOh9ChBYCd_XXZaaUMc44991xsS487QW84W4E8zZMx9hmJuzVPnYrWywg/uhm_Q7EEB6UxIE-aRVp0yaRRgX5b3Qswfv0t8JZBjGo\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/8gY54C14V6hkctOxYFi9jw/rDl82T8e8XRjQGdV46JM5oBCFLpzKYDB4I4YQhbRilLPsPtRfvo-rkuHmOvz5VoJ2Rksj1JSur4Xr94nCc6_bw/oYGJpcqKaG8XZmD5ictdRsVr-tKixhBajfDgTjdMwzI\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HupXpyJP0RMjjfULl3JRrQ/eZg5CfzE-d7EBZmRUDIhv6t3-bfqWWllXlYeufGfTxpwTH44c5dD-hZGvKYBHgNA/AxWJsigWpfxcjPtgHkAP-KVO1SkOAjP1YiI81iTRF3A\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NvQxlM9X5NQW7zOp4AOkTw/jW08CtxJQoA_rq2EvQOstuFPe-XpvikDGRhnjCFow61fqcYFft3CDz1Z2PR1BPgcNGfUqLmTjg-Yf7ZWXUtRYQ/H-sIC8zzV7Bxdf7LwYKc47zRL7n4gcAyncbygJskoAY\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1WwjreUt4BcwqqGu1_jlMQ/t3kmPFVWdlgI0zO4cyzm_5hnhAcfzJT-2p6LOHWZLd14shFRhoAfuZIAb3_UCo5ZQyrxCmHFFdkIzOrgK_Hw-w/I5C5EcuW1o2qzBGBQAuseP9XerIshRbFpsFn1xG-qHs\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Zg0G5_M_jwKtmw11dPPCHg/HEsRVXchLl8s0X9WtwmUAELkhTOL2mYbHTuCsqlwHpvSggYzqG--je8Kaqix-rH4D8uOzhFIYrCJbdOlCd349w/oaKrXWTLApH2eu4Spay1lOvIcfUCMgyeBrTs_xozCxw\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/GoJsfjcQ-dgcFx1F2d2qUw/tj4FG5snjzxkrAEI27jPzHReh5nKUm7Z1z2k9k0n8bm_ul_LRg9dlj2mfcKRk4f5/i5SZKJhHR86GKvzy-W-9tgr9dnN-jfllmnWyH461Z2A\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/7qrj9d1-WI6_IqprUuL63w/-kmzYAaVG8boiUupuKRXUV2kZQssGd-MVsnNb4nuUAktAyxTCayh5oFrkTp649TJ/D5F9EAY42r6dW5GTmmbZG9WPb6lll_yco8cPpiNAvuM\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HanQcO5l5itrnEoJvOpM4g/sq4hz2Z3vGUxNh35QRqMlRyNqZiyAQ_ghGjruuWpmCuU5QdzD32EUT263SqOsS4s/4eIhoSgxoOqQX1I0Wq3_gTG69ukcJeyEu0GBE_uQouQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/c93vRr5teXAGgw8p3WgPaQ/vxq-24t-JVpe_ZKg25DgAWn6k_tIkufS_6_mFsUGC2PbwyOEIIirwNMvCZTZXhDD/rzILsKoDIrMeIUahol5vFXAURgXil_uBERTaGkcR-qw\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zlOnaEgEDiB86LJaowingw/fA5SyRQtzRHtSBEMNJW7kSwq8GBtv1taJ1EFxRoiqqKBuri9V9aL8_0yu1Lpka0A7Apr1rE8K1BLT0ybBDBwRQ/tZQdKaGcqkRm2w1fvD2hWUbybMlEgHeQc9K7n1cu8tQ\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Kgs_SvSqybXg4M1tZJm5cw/QkVeqSQRN-sorTaB18pPaTTS_c0UXcTlWHz0zAzUR1Il4rXuRCREh-eqsiHq6UYSLfIP0VpgZPC-bbzKPf_ozw/DPgkFtrHUGhdcNzV2L9TRekA2qZLN-9kS_gAP0fTUgA\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/OpWL9ql-qRhYocT5kfV92w/XgElInhWNE8WcGUjxeipYp0IbTp9E44Kk8PdV_gQr2K9Wf9jN2p979qw-gsoCYgvylIhcU4E0_t53UyAuKVj0g/V74yqGMKqAPZN9w6zVwP9MaAPmbx3Al-il8U6c0sVrM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/V4ytQHf8bCihnMsJ7fMyiw/a3PAN-omfhxWqGxPRctUEthLXmaSkY2ZuLFtK7vD4pvXFupQ7xmWr3CKvyNEQZ_tAnckHKxCls9SEuLhMWwtNw/-s5YiZfOw0uKwCWZ97jJ-Y_8ieB36dhC-hil8ZHivhU\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zk3ktaxvd7aRG1of8BbOtw/MU0udt_s_iFNkVkw2KJwm2wnFdigLqQcWB5J29dAt4Ot1hBRizcO30tTfXDHkTf6sRpHQ2652SYKm9gbUNmGSZbGSCIKZEc-HF2cAQoJWiVqV-3xA6U3iaQkXJgfYniznYaM7J5WoeHL3JmcybhAtQ/3lJBdKA4SIcY0p2QyUgYw_F7aiCuoUIlHfWIC6v6Zps\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UwK69wQ8ZpIIkp7S4mpgEQ/EhAKnXfUtEigaMZ1--HYltrQRW_XIa2ZYe9XpUCNulVfLvv8yIRnf25YfnxoK814BpX38ZC65RKYtc7xDikmYA9LoIgOKwlo9z_9yqLwy2R22tKd6JilIcitNlI2H81xrrA2Uwz2tYhNOEimk80UsQ/4iy0EuV8951kQpwM67yAD2wRmHG53dErnD1lP5yUuwE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/T1eMM58E2Up60PALN-hIqw/_oYbjD7N7jImNwIfrPapfQ2YYcOAQSoGnL38hIsgdJ1t-qSDEzkFlrS6C3Lrx_2l4OXw_VuvK8TeX1R1Fcb6bbEy-JlAJIH0vTrr03Ct3EfptXriBPPpODPqtx_bMPHW8s4mo6CxXZRd4waEX7_qPw/HJdWG45nQDMGu9kK8pPXaOSYqEcB1YcQDgqqF1f4dRU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/u_3HMusgY3FmD0ntOTTAYw/-4vgpJgLKpaaH3WoiAW-XkiD20FXlt7eEzSBrnioT6F0rXIdXv_fGV_Edi8Z7ATW5ZNb70id-ASy8X2aZfenLuG6fLAwiyXoSib1kGF295eDQeYpRqb1eraQB4gPESadDorC-b2-j2KVIv2hYgkgHA/6pAadbXGlqKznf_n3_mrhVz_FV3TBZupiMZoFGVcc-Y\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/v5RRKQ2rrnaT7BpG6YxUmw/zWxV8QqQyxLtGH-wlTKTGdZAHM3HRqzuZ74rxapRfEvBlnUwJVho3_Xpk2_1oW9h0IB36D5KbRm4CUxM-eIRgQFMpY388-FgwQaOU5KkA3ooUc7z4AEgZNIH6DDKyincqfmw877fxGS5d7KpfwM3UiaQg8dUa9vt-aGFggnZWwodrcK_yO-_vnKqKvLd3Kk9/YJHZ_QmdwR5c_VB5qGD9lfOhjC3-ESOSVLaohvrGonU\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/6P0256TFuxT-dEjega0mcg/VI2e2WYIq4gWXnNIB_pBp4OXy3LF_OzCJF99RRdXflVOej6p_zLq2R6L5KWsMDOAFhqrbTcFJ-yt1pUdVKD7txrKWbb4TBCVXTPiaI3WG-fuysulumFqjgkhYSjv9UtAJNmqrln4KGPSRhwGhBVkh0TnAcUdWdsDiqRSGG29ZTnUakc2s3fWWx7nkJaUOpec/4owxPLxFK8qOiXKWCPljACDN_bNTycfKd2bnpjdirPE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1fzFnpP0RTA24_QM1glSbQ/4IrmXRvQ4jf8_MprceWnFX94gHzt_9XTWrhvUEkfeOHY3vqxe-fDnSUjW24PPeQROu7Vef2pyk4ctHGd4_mMc3EqnjvmTrupq1b_Z2jE8riLiDyqtCp1NntCWZJJP5B5kNOVIEytsqt-OJ1RHCoARF-kE-AKfHhOWUQxGr9hrQYDfS_E0HO7A69AdP3Y0N4F/qJ84hKf-ML33Z5W8iWO0M8Mysuid-00M3hQPDICpCzg\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/JfTwylMJVY-QmcZkPVsfSA/FP7ATfIkL5SOAHa1Cq6DylMn9V4q8wwMMlwRBYM1_OE2Agn7fB6PtxS0V0aaCKszQMmCO3nU5ysu6S-m80PoI8K1ojuMiGWC2QSLI2rX2IJuzlOJBuCX8kp04rbVYjbxVuyWk18Cx7cQnnhYFuaYa-n4CVSbKrKCNqYI5CvQAYaRSE8bgmGpoKlMzrBt_KZZ/Jk4gxMYhLz5qQPs_hSNszIygOY8ef27xyk_UzmrbsD8\",\"width\":600,\"height\":600}}}]}},{\"id\":\"recaaJrI2JbRgEX5O\",\"createdTime\":\"2015-02-10T00:15:45.000Z\",\"fields\":{\"Genre\":[\"Abstract Expressionism\",\"Modern art\",\"Surrealism\"],\"Bio\":\"Edvard Munch was a Norwegian painter and printmaker whose intensely evocative treatment of psychological themes built upon some of the main tenets of late 19th-century Symbolism and greatly influenced German Expressionism in the early 20th century. One of his most well-known works is The Scream of 1893.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Edvard Munch\",\"Testing Date\":\"2012-02-01T23:36:00.000Z\",\"Collection\":[\"recwpd7MLPQqorfcj\"],\"Attachments\":[{\"id\":\"attNIEYhExe4q53lp\",\"width\":1000,\"height\":579,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/bFflpUQdYiwyDyiknmqmAw/V8gThhMdkvKXhgrxXwa5fenZltQfwdTKHvWFzXcg7hDsYPIAZ9MRyYY5KakPeGkc/b-lQZtSkZ_tdDTVooD7b-EFqmcTvnfv1ywvD-96QrsI\",\"filename\":\"The_Sun.jpg\",\"size\":194051,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NF2YFUuzfI3T5NU-AcMhcA/rJyRsmGKbFZij8DmTigCfXaQqG3SvPji6XfGE5Gpaxi4l52S5MdWziFvAcbo2heI/i7zKE2mzUYtPW51Ww9Wiplc_7DZoQxiTCdJzM3FZH1Y\",\"width\":62,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/6Ce9TyYtcPdmc06XWEAlyQ/h0OucuxEeJwvxSEG_ljmEeTXvOtplcNmeQXlj-0VqUIZlwg_V6k11FLdh45Uu33v/KPy1MsN63B4nfsCClkaxNvcfq_OusegI-QhOWcjnj2k\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/_i-rVtbxkRNWU5P9qSrzIw/dHGCZEx_WouJiGP-ZAyRmtkIbKAUd6M-Fa2eEjM8fEFX-Jo_taHZUIKzPUyT3b_O/E1PCSOQ5xBKJhgf9nkAnL_IrtPs9I3lKM4vhXYliy5c\",\"width\":1000,\"height\":579}}},{\"id\":\"attVjzN5X8xdWoc2W\",\"width\":1458,\"height\":1500,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/jRPv-_oVfIfcuJYnxwFYQg/mvnfJlEXCKfxXeZgRNNAf5ON4FlZJngPAhRzk6Ce5LvQ0X9VWaU5RjW6tguJPDUhjQ7qj4eZrINz1lFUxpZdIg/r00FWA-x349oikEXLw4fcufgZtpeRMN2x8haynn7Knc\",\"filename\":\"Munch_Det_Syke_Barn_1896.jpg\",\"size\":425603,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qTzsaQ3k10VyLOIYDk5_lA/M10OEbYi9Z6TungP8bHnXnOF8Ulhlsvgin_DmKIFasmoSv14RKwF_DD5RO0vK8GyK_HliPOh5lQTny5FRmbRXg/XY9Sfs9QAp4uz1xuhVabtZyaqwaVOFBAHu1STaZVlPQ\",\"width\":35,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/v6IGncr9VIzFytlfYaT7hw/JTzafGYZGr5-sG2W6SBkQpN9UFNaUCZj7wwxjotoCnKaFj4fF5mv78ECoF7O7QKvq-Y8OXDX_FwTIvNB1JSbkQ/_ZFXDJ3HnVjUxnYv_rFaGV6Oj1aZz5ppD--cdMClw_U\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/k1Am0kz48sPl0i0aVIswsw/lBN75X8Ip2ocW7V6_LBy7eIhPdwJnJzbHwi-CFfmrB4o7c5SSgIuzqA3f-zk4srv0-rsEnRncLcP9DlJ-NDJmw/StVJSyZNmrYkWub9yTPy6TTyAiecdL7rs4MCGY_doW8\",\"width\":1458,\"height\":1500}}},{\"id\":\"attnTOZBfCiHQhfy1\",\"width\":850,\"height\":765,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/jw-bgkiH2-9Wy3pqJWOUVw/zVaw-zAHE6ktzVU6FZ_JDW0mnyhG7QMMgdt7W38snT392JoC03A08q_VQ-CKZNMp/J3_DYSuKEOHgVOHV5v5NQxJD25OX6UVAeHRyVve62Gs\",\"filename\":\"death-in-the-sickroom.jpg\",\"size\":255101,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/vjWLTosrwyrEYNZtmfnjqQ/wKGNBcBvY2ZXxSoHUn_sL9xgqjKYmbbWu-MnitlKxtzURMHWsp1tm0g0c86rNta1MUWk0XEL60N6OX2o7Y9z_Q/c5cQcScaYLD9Kzn5vIhLWCE3jGWwiWsAbGv5pZKTvBo\",\"width\":40,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/G3hVzPL1D28T2DCVC9F1JA/WDKtHPTaTMV0zPYvwjX7kX74ucpMLAdLS4pkgIifMMWdImYwO_oFe_-KJKj1_mU-9KZEobGi3xax-K3afHgu9A/6OlpctkXtiAQeaFNyoww1aEof9SgOu6t9HpgkuSsBjY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zRCpnxCbMzoDT5ZzLGV4NA/zLMQzteVhQ6AujhCuyu-ew6T82oiXD2EiCjxjMQbe36A92haxZ8Q8UL53qIe0yuNuvKwXXtNN9WLH_Ty2Qeh2A/801oFPPuH2dHT4P_jvybpkczLlZM1ybTgZ1Sk9-9zOY\",\"width\":850,\"height\":765}}}]}}]}");

            string bodyText = "{\"maxRecords\":4,\"returnFieldsByFieldId\":false}";
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            Task<ListAllRecordsTestResponse> task = ListAllRecords(maxRecords: 4);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count > 0 && response.Records.Count <= 4);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TgAtApiClientViewTest
        // List records
        // Use the 'view' parameter so that only the records in this view will be be returned
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TgAtApiClientViewTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/39oBWza7hOYJJgaVt_yRpw/gaJUvLehOOfXILepGr7JiefPNcFMCedzVWknVqxiiRvHI9zqiWWbDW2OeIy7bnVj/OTmb3wZFcrc_MX-93zh_amU8azw1ROc9Mi-AUFvoWQ0\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/f4CQhJC03yufCMOak31dbQ/vwF_STC9EgXFsUkzCY-KG17kQnz8a-iDmBuKaaD3n-8pzXBJAvtDpnczqkuGtVTpYZcV3lYGLY3iX4qaiXnJqA/RlDEyWen99f4JeSuYurxTSFKZNlDd4v5tjYSoSpG2vQ\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/g48Aovs5yGIf2ZgLZwH2BQ/eyoJRrpw3IE2DxYX-_RP9DerRvEnFhPVU-Snqb684bwRb6qmRCguRRcBmszs9Es60AcrRCj8d9vR3ps7_xRSPA/EG_wDl7vmfQdm91Lh_N7IM0B87G-nLgkcsuALExevps\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/20mlEBWc6Bovof2ASIX1Zg/coJPJ4VPg24Q21fcIBI7SK1c9KZYW3zhSBDNORjrGgmDC_smPFerLHOVZOcBOUCPIizTpO9RRgdjf0VqUuJAXw/syA1G5xMrsCLEyR4VHnGss7n9sIqx-gjWPSOco5QFHs\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/I3-tYuTmDu89tn8_683myg/VnVWv9cniMd-24LQzZbNCbv8Y8LEdNcx0ngI6eHvH-yTE-zUJ8OWCBARH3-mtjJL/e-vhSIglR9avANBAL2rg-uOblPywKQ84TRVDNwiEGT0\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1Zxxi0BDpYmsBKka6hWATQ/C7nmfnT2nHHiFxQZEGa48v7bYDWdN0ISp5M2YT7klzBT3UvUjhGvpgvDRKBBRtm6/DHlRiTDu3Y8XtcAB0tUdvM_ROp-5W-yYNmGBxpEj7SE\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/TkpiadKNPKchJFC0nozMUw/7D0P5PjcQiHxt-R2gcV4ECwDwePR5t_3pQ-3V8Ieb0bbQEAkeKE4eHXqoawmqk7p/0Nj5NKoqhJtCHGiKI6Bapn_mUzI19LsLZnbKXEgDXz4\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/mAcRerRZUjZgNN0MMIZK7w/Drvhmq1inhnqUJaTTxiOhedWpjRHksluhv88-GXNm22BO37fLmbQszVSyyCL1syw/zNh7DmoMdPdDON8BgfQRSfabQf2XrOrjF1IRzn2xE4I\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/LJLILvRGC_OOu1kzFeclfA/8o3klF3vxIzq1MBJH2DH8RGvQTTpj9OGTdClHm78bgh4T7Xr_MbFAq0EGwo4k9sh/5j_h5ZCEmwPMSvEcdXRSLOHMaMdIM2DkpF13sfOAVog\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/tKwV1-9jqI1NfAlzp1mC6A/Kdnz2ID7WL5LaxKvLtY5mUG8NWZ-ypzsroTjiMvi88biBOPrCgfoVLBRGjsi0UiJ/AKXvKP63YcFBV0v5d5Lszmx7So5Nnw0SxpAFN1QYe74\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/P_XjuIaj3FZPiDALCLc_0A/6C4ktS8WKbnlSA1pmxKJV5yJS_AjoVAqhgSz2u7kSduwFGgY6B41WFoOgw58DaH0/36wu4pZXdqujq3GBu9tHwzQ4v0lqSnMry9hibfmWEnQ\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UrKyE50u0_m3DXznTljomQ/IqZyGe0im_rdzvvw8r4RkeiClqKrSARXR3CrebsI5lo4RnQ9HiN6hJyaFYM6J6lU/eTYwF5IhXhqqqdORJVEUKvObU-g3OIGWB_FbYXP7it4\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/AFMISdEguC43D-e2t6BRhw/id18VPoJ9MoFmTl-cwVkq7F9ifIfstM_wwhkkTaT4Ce0OSa2blTBzj1Ubcco0CeL/GJ4aC28m1h-QBvdhPtjZpr9kM_2AqxtvliU0ehwFiF8\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/r_vFrkyQ64TZJHLTmPAmlQ/VSDxC6OwSu38B_p40dStq5CaLkPZgdKmD7rMPHbco9e8ErnVIWe767wNT8hHkThl/YomqoVD18mfLgT3E8rCVHy64gaXWBkZxJzeJeGFKuA4\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/fEaI7AaMNNosIfAR3MI3Zw/NIS5mLHKpmD-3NAVhQhEZH8uRV6CT43bn4ndngoc5r63X8FNcE1AQ_YqFyKvcA17/98TzCdD66Ug2lnWLmjQupsqJFWlrfMJo9nyAIJ3FUOU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RVdlXPRU-sF92yAU8zHM6A/vkpdS8zx_NvRBSDlxSNxu5-nBiSmKbz3r0hj9-nGXybutuAeFfm0oY6x188TW6EQ/s8e1sWCvynjZ9x9MXBe5mnQUfWaBRDdMkVGko6yywoQ\",\"width\":716,\"height\":720}}}]}},{\"id\":\"recraBPRF3m5Te5Hn\",\"createdTime\":\"2015-02-09T23:24:10.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Mark Rothko is generally identified as an Abstract Expressionist. With Jackson Pollock and Willem de Kooning, he is one of the most famous postwar American artists.\\\\\\\\\\n\",\"Name\":\"Mark Rothko\",\"Testing Date\":\"2020-07-06T17:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attONu0jXlWNlHOxh\",\"width\":213,\"height\":260,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/PbUV3GShksF7eS90_rrBJQ/jM8_jTwippopEtSruqAyKnanitl7tSTSyYPLJox7LnBkgdRMVAYbGbuNfk4cv2UD/GIugB_QeOYiKQlZhbceOh5Cy2UcO2sfDCT8hJO-CM68\",\"filename\":\"No._18_1951.jpg\",\"size\":7416,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/dnTWghS_CLbLPErUdpkYlw/FcQvNKRB0IUQfS_OUKiPhZiMK36nI5uh2gw-MRxAH0SiW4bnY4YjHa9dc_3F2hm8/yu8xDg3MnpBP47geWMbMApOBamBdu4GMLNisTHSw5KU\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Owm3s_T0ClZATmWYh8Yd3Q/KNsh-aNVWjL63-eAeeiKCjCNthx0OboZqnzNZTeZlIR2rG4djaZuoYI2rchAuU6i/gWGUHp15-bh2QVVTe3fU7-4pFv_69DbvA-RZnTypZ80\",\"width\":213,\"height\":260},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/QFT0oniW5fRt4uerd76_UA/dGGo5QWu_PJH0lmYBvOp3YLMuGZprv6uRwJckR64FB3F5sph6g-fpdJDWUWRJN_x/yuce_4yYLPycwODVjZjJrPoz1x7y1ERdwS4vVgmDyX4\",\"width\":213,\"height\":260}}},{\"id\":\"atteYo0fXP5bOpxt6\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/jy6-f9zfGfaQ6GfXh46Eag/8hAPj2fzKgkSkBaTc0tgHPY78yf9uf3KOOX-vimCysVIGEcyZisxIWl0BJlrL0Mi/40UHv833uyx9BnIMeBQbWQm8SHHKZVegG8kBp_OEph8\",\"filename\":\"Untitled_1954_RISD.jpg\",\"size\":23636,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/BfHoD3VhEG2sfKySKfYGpA/j1ihCP8vlaM-YLo4AYJROZrnm9uf6_IW7LAEWOjIBLzw_X7EGWbxDrsZEM6FrupX_YoCc1EXHEtBXr5l6jLh3Q/eQF8yaXoqMTV94o2to6uKCDYAYc0jglFKbMe-t05_sA\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/kM4zjv9Kg2rQ9R2uIKsBVA/vXY_-TIXn2v_8rc0EpPnJobnNejdiDyubBNa2a8FXcrMTChgY9r4IdyMqs--NkBrsVg5eKAgRR4MqCfHRti0kQ/FDuAxDUXBFjTuIFdIWx7bo21iBfmR6lZsZwSnxg90d0\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/tpxRiskTPIUHcFwamGG2lw/YhzPHu_kgmadbBr16ZSmvtnMainogo-e6qjRdyv-yQQ7if6hb5Qo44r0Cqhftwo9puhko96HE03tA3GMzOcHPA/GgkK4JDHGox8xsjx1tfyFM9_KO3yp8UAQGH7j1t1XJY\",\"width\":385,\"height\":640}}},{\"id\":\"attneJn9hR5DtDns9\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/ek8PNYj99sbtEYG546HJjQ/M4lTD-70Sp7ZW2ySxFOIr6mHG6QlZZK7476OZ900ABMW8YudhRz77ELOnt5jxS5i/0TdF7mfi6b1lTBRfLqHZtYbb7nEceEjsS3QmcYqEsiE\",\"filename\":\"Untitled_1954.jpg\",\"size\":33352,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/ksoZ22Zhvxj_GqWuij14Xg/dD-UyXwVh2gmBZlmX3fmHSCg7pFYBVsuxUi5KA_d4rmPKpab8NBdLsh8kn9FCmcQ/J05MFpNIYBRnsBksyWTr0yyk3LgMg9VhniAmRVfHUY0\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/QP6YIS7IA4qxDllTCBJJXA/IV_nNgQjbI8-5kNPTnNCbq4JZZqsCh2YLnmiXFK81W_Csqn48n1nZNLScRKAbMDy/JtvnBui_46sAsKMJcl06QIk3ChiEhZvjSwY3UaDKbGo\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/70hsT0xUHiHo1TqyspxZAA/FVvlgrYmT3-TL8LxkYVhAOH5EqNjgwLaD3R7NYOb6IW0ijZyl-MqxBFEdkExS1TW/TnwzNpwwlH-0phY_bJbVsNqCV1MTN_USzHtM8044ef0\",\"width\":385,\"height\":640}}},{\"id\":\"attnicjT3NIYNL5Le\",\"width\":551,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RCSRZQDTEcw0hSma0CTf9Q/5J0xZocW3v2EJmAqFMysSTDwAgwLd4pGLmuFlhb2VyQy3v2YxdCj2bl8IoT5iNEq/aa7M3e4EXDJhVSMyouhlY9BfTy4zV_T6P1WhBXsrExw\",\"filename\":\"Untitled_(Red,_Orange).jpg\",\"size\":43346,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/MRK_9Zfb1-QoyAW7_fYjag/z5o01IpdLlRHExEp0-BV_YWiswTqh75ql2Y9PC98c-LIepXOVaEEWyU2MbkZxaEKkac62ZhQoHpuynIxAlvTig/OkE3ZIBBKAvMMtOqai_LEbXoHfPgvpMHjUBT6d2qhnE\",\"width\":31,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Nrk9zEjCb--X3e-CaDzHWQ/yVIFV-bnqEeAoiFTd8KC4UQdQxEBch7NUaRo5MrbxTSRoGtE99jlayyntii4qrjLjUMpmOFjGlTMBIuRxIF6wg/KhyOEj1x94gBLsswCwdfdtV80ylMdKtVKKcXODlO2TU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/KR_OHMXJgb2LTPg2FE0nqA/ZMjgjzDe6d1k5TTtkx1vzIk8QkLqriKh8l0uHYVGcc3vRcWUrynaJdVE0IM4PMf0UHoF52ImAUiE0DbxYs46CQ/Ms8AceaI5PADU3NdQ76qKZ7FROBEOHNDFNFVUKEKAxk\",\"width\":551,\"height\":640}}},{\"id\":\"attpDkpjaf734NjM0\",\"width\":480,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/4EliRVvEbjBXI_HG8UKJYg/2HOaXvOKkXb-VlV7xjDFmvb1ecqnEowy2TjwgBV37Vnr-hkVV-cl1-VuUfENaZFn/Oq5LoXCt_Lew6HzFqpzpxybNXiyDlLTicbu_qJudK9U\",\"filename\":\"No._61_(Rust_and_Blue).jpg\",\"size\":35819,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/F4W1skt82SPkKT6TYTj8fA/4T_w88TdVQoURMCX4kIwrMiwSOPgziQ2d_x53iJMJzV_xw6cHHagvHACZ0cn4C5hmupL0b8TXcV6CGt8BhS8IA/z_DKt7vrWDAG3Dc7pQxRtm2ZMaX-frUt-ne8J8AzPDc\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qhM_BCvbZIOd9QxppXnevw/0G_3GP_unGeDXRX4zu5kUfaMRYsyYMjBeeGg0WDxn9Tg1xRiieH9mlgdwcm5oT1Cxp4tXgx9RF0dCBF4Eky61A/4R7uYoLEKjUxZyUVvsBAZGTIf_oiUn2xEcDwL47Org0\",\"width\":480,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/aczP_KWUlOjTx4oWXPFxRw/CgTAwAwDUtklKVLb2Qb69IvgWN_vI1ZrVUUMCpVkGogpAwymvOKWT8uR9QMD84XeajX9AoEKNroA5I4PIgWdfA/xNOe9bfOoAimon4Y579IQf8C64Qa_SIMWjCBFVKzw2k\",\"width\":480,\"height\":600}}}]}},{\"id\":\"recaaJrI2JbRgEX5O\",\"createdTime\":\"2015-02-10T00:15:45.000Z\",\"fields\":{\"Genre\":[\"Abstract Expressionism\",\"Modern art\",\"Surrealism\"],\"Bio\":\"Edvard Munch was a Norwegian painter and printmaker whose intensely evocative treatment of psychological themes built upon some of the main tenets of late 19th-century Symbolism and greatly influenced German Expressionism in the early 20th century. One of his most well-known works is The Scream of 1893.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Edvard Munch\",\"Testing Date\":\"2012-02-01T23:36:00.000Z\",\"Collection\":[\"recwpd7MLPQqorfcj\"],\"Attachments\":[{\"id\":\"attNIEYhExe4q53lp\",\"width\":1000,\"height\":579,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/bFflpUQdYiwyDyiknmqmAw/V8gThhMdkvKXhgrxXwa5fenZltQfwdTKHvWFzXcg7hDsYPIAZ9MRyYY5KakPeGkc/b-lQZtSkZ_tdDTVooD7b-EFqmcTvnfv1ywvD-96QrsI\",\"filename\":\"The_Sun.jpg\",\"size\":194051,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NF2YFUuzfI3T5NU-AcMhcA/rJyRsmGKbFZij8DmTigCfXaQqG3SvPji6XfGE5Gpaxi4l52S5MdWziFvAcbo2heI/i7zKE2mzUYtPW51Ww9Wiplc_7DZoQxiTCdJzM3FZH1Y\",\"width\":62,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/6Ce9TyYtcPdmc06XWEAlyQ/h0OucuxEeJwvxSEG_ljmEeTXvOtplcNmeQXlj-0VqUIZlwg_V6k11FLdh45Uu33v/KPy1MsN63B4nfsCClkaxNvcfq_OusegI-QhOWcjnj2k\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/_i-rVtbxkRNWU5P9qSrzIw/dHGCZEx_WouJiGP-ZAyRmtkIbKAUd6M-Fa2eEjM8fEFX-Jo_taHZUIKzPUyT3b_O/E1PCSOQ5xBKJhgf9nkAnL_IrtPs9I3lKM4vhXYliy5c\",\"width\":1000,\"height\":579}}},{\"id\":\"attVjzN5X8xdWoc2W\",\"width\":1458,\"height\":1500,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/jRPv-_oVfIfcuJYnxwFYQg/mvnfJlEXCKfxXeZgRNNAf5ON4FlZJngPAhRzk6Ce5LvQ0X9VWaU5RjW6tguJPDUhjQ7qj4eZrINz1lFUxpZdIg/r00FWA-x349oikEXLw4fcufgZtpeRMN2x8haynn7Knc\",\"filename\":\"Munch_Det_Syke_Barn_1896.jpg\",\"size\":425603,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qTzsaQ3k10VyLOIYDk5_lA/M10OEbYi9Z6TungP8bHnXnOF8Ulhlsvgin_DmKIFasmoSv14RKwF_DD5RO0vK8GyK_HliPOh5lQTny5FRmbRXg/XY9Sfs9QAp4uz1xuhVabtZyaqwaVOFBAHu1STaZVlPQ\",\"width\":35,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/v6IGncr9VIzFytlfYaT7hw/JTzafGYZGr5-sG2W6SBkQpN9UFNaUCZj7wwxjotoCnKaFj4fF5mv78ECoF7O7QKvq-Y8OXDX_FwTIvNB1JSbkQ/_ZFXDJ3HnVjUxnYv_rFaGV6Oj1aZz5ppD--cdMClw_U\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/k1Am0kz48sPl0i0aVIswsw/lBN75X8Ip2ocW7V6_LBy7eIhPdwJnJzbHwi-CFfmrB4o7c5SSgIuzqA3f-zk4srv0-rsEnRncLcP9DlJ-NDJmw/StVJSyZNmrYkWub9yTPy6TTyAiecdL7rs4MCGY_doW8\",\"width\":1458,\"height\":1500}}},{\"id\":\"attnTOZBfCiHQhfy1\",\"width\":850,\"height\":765,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/jw-bgkiH2-9Wy3pqJWOUVw/zVaw-zAHE6ktzVU6FZ_JDW0mnyhG7QMMgdt7W38snT392JoC03A08q_VQ-CKZNMp/J3_DYSuKEOHgVOHV5v5NQxJD25OX6UVAeHRyVve62Gs\",\"filename\":\"death-in-the-sickroom.jpg\",\"size\":255101,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/vjWLTosrwyrEYNZtmfnjqQ/wKGNBcBvY2ZXxSoHUn_sL9xgqjKYmbbWu-MnitlKxtzURMHWsp1tm0g0c86rNta1MUWk0XEL60N6OX2o7Y9z_Q/c5cQcScaYLD9Kzn5vIhLWCE3jGWwiWsAbGv5pZKTvBo\",\"width\":40,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/G3hVzPL1D28T2DCVC9F1JA/WDKtHPTaTMV0zPYvwjX7kX74ucpMLAdLS4pkgIifMMWdImYwO_oFe_-KJKj1_mU-9KZEobGi3xax-K3afHgu9A/6OlpctkXtiAQeaFNyoww1aEof9SgOu6t9HpgkuSsBjY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zRCpnxCbMzoDT5ZzLGV4NA/zLMQzteVhQ6AujhCuyu-ew6T82oiXD2EiCjxjMQbe36A92haxZ8Q8UL53qIe0yuNuvKwXXtNN9WLH_Ty2Qeh2A/801oFPPuH2dHT4P_jvybpkczLlZM1ybTgZ1Sk9-9zOY\",\"width\":850,\"height\":765}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/dyGthKoDMnKwSI2KTA6wVA/UMVA2dQ5RSnfjfNlvN2--fzHihEvEsSBIjd9e5DH9TmNCyrrPgnyibB_aRuxpQnw/7g23WoYII1RjnZpHinLSkSfw-Dx_2zHuCTjpYiFm8ic\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RhEEH9XTSra1BtNxqAnztg/i7j3E9P8ZZs_CIJWr4ZHS6JHHgL--aPY9oiuf2Vee7BON7AQ4N-YXpXpbEIo4rxe/47rBsXKdVYGcTTgVnu3FXtnmZ0DyF8OXnpzmvPIsAfI\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/df87qbzGy6wJIanNCyBtrg/xUy28IShaE4kuXQ1qv9VT_xi8NMLRJNHGnowu7-axpnXr8mdoKjAFsLAvSRfL3UU/QqaimoVqaY-uqYPEac5fzhpkDZfOFZ6RZg0qIEderAA\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/rVIShNrp_34ji6XuooCMeQ/NnyruykjDN49ZGydPKfhe3CCVFdvyXZFYdXG9pqwGHET4AOGuKO_zYveGxZHzpz3/WEw11im0irKutvWzzuY1QroU95RzCr4RuwyksPha854\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/5oVv6rSzFGnN19MzVCL7pg/6_qT7KCq4jztukRdwBG17FAZFWBqkb_0pGwAC1nsn3bORCFwNXYjrWTzrLVP7_RQHwj98_FOgIyv414dylnmdQ/4F1nzs-VWwB5nW_zYG8eCGjw-JXuNWCqnp9fo_WozfU\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/nVkcUIfjWGvko5xzvtB_Gw/Vl_T7OaOV36M267RTg3p_ymwc5FMzj6vySE4MXkDtkRJjTWnkDjVvrleKNoQIJ7YH6Lhswqvp0LK8s8TW0CAug/hWdhfxnxEJ-JpWXibAArUE3ZoKHaiSas2XI-J-YNRz4\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/J9LtV7PbhIe_l-mgC8whVw/kwSbJnyHqWwdWc7GY712d3SgS-BKiPOh9ChBYCd_XXZaaUMc44991xsS487QW84W4E8zZMx9hmJuzVPnYrWywg/uhm_Q7EEB6UxIE-aRVp0yaRRgX5b3Qswfv0t8JZBjGo\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/8gY54C14V6hkctOxYFi9jw/rDl82T8e8XRjQGdV46JM5oBCFLpzKYDB4I4YQhbRilLPsPtRfvo-rkuHmOvz5VoJ2Rksj1JSur4Xr94nCc6_bw/oYGJpcqKaG8XZmD5ictdRsVr-tKixhBajfDgTjdMwzI\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HupXpyJP0RMjjfULl3JRrQ/eZg5CfzE-d7EBZmRUDIhv6t3-bfqWWllXlYeufGfTxpwTH44c5dD-hZGvKYBHgNA/AxWJsigWpfxcjPtgHkAP-KVO1SkOAjP1YiI81iTRF3A\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NvQxlM9X5NQW7zOp4AOkTw/jW08CtxJQoA_rq2EvQOstuFPe-XpvikDGRhnjCFow61fqcYFft3CDz1Z2PR1BPgcNGfUqLmTjg-Yf7ZWXUtRYQ/H-sIC8zzV7Bxdf7LwYKc47zRL7n4gcAyncbygJskoAY\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1WwjreUt4BcwqqGu1_jlMQ/t3kmPFVWdlgI0zO4cyzm_5hnhAcfzJT-2p6LOHWZLd14shFRhoAfuZIAb3_UCo5ZQyrxCmHFFdkIzOrgK_Hw-w/I5C5EcuW1o2qzBGBQAuseP9XerIshRbFpsFn1xG-qHs\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Zg0G5_M_jwKtmw11dPPCHg/HEsRVXchLl8s0X9WtwmUAELkhTOL2mYbHTuCsqlwHpvSggYzqG--je8Kaqix-rH4D8uOzhFIYrCJbdOlCd349w/oaKrXWTLApH2eu4Spay1lOvIcfUCMgyeBrTs_xozCxw\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recj31Rc5TXAiVZV3\",\"createdTime\":\"2015-02-09T23:36:53.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Experimental Sculpture\"],\"Bio\":\"Isamu Noguchi (野口 勇) was a prominent Japanese American artist and landscape architect whose artistic career spanned six decades, from the 1920s onward. Known for his sculpture and public works, Noguchi also designed stage sets for various Martha Graham productions, and several mass-produced lamps and furniture pieces, some of which are still manufactured and sold.\",\"Name\":\"Isamu Noguchi\",\"Collection\":[\"reccV1ddwIspBOe4O\"],\"Attachments\":[{\"id\":\"attuoGtQSGoeWEurX\",\"width\":640,\"height\":487,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/yZTgKAaWcwfCExKnoEB1tQ/HoJrgwi6NUTDRp2qLH770PeeL3XIa4I5USmX9WR9hWc/rvhVZFT7v0RIwBMqsn2rvmpqh-4SUmcmoPbFkjXJreM\",\"filename\":\"Leda.jpeg\",\"size\":55738,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Xm984HlTOt-Evm7XypPheQ/M_yOhO_xPhMQ7vqJeeocbIBuy6hQqaTDSgD2Ttf9Ajbnc-mlAR9ESq1S0YMHYua5/lN29NGXCkjvoWJ6rTPtLXS7ujrqZW7oYtJOXhZCE6tE\",\"width\":47,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/TooUBGJwwGtHiAloT48u2g/jmA5DW0O9er_mBrNlus0Ehly2PZk5Zpd-7sp5b9GyfZCLaD-udl7ZJTCY-hwLAjm/TreykLTJP_5xSE_OH_sa8uwX9V0wOj8fQNt_77lMj9A\",\"width\":512,\"height\":487},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NWGeILM__eyRrhsYgG3KQg/9AP9mOkSq1zGiJOXvZSV_hvdVgoSFKIKbx3KPLcDolxqIHie8y3W2eTl6tLE9gPw/xSyZztnKIrLt82M1DGH6r-V7RpbFnVjTgn715Fit9r8\",\"width\":640,\"height\":487}}},{\"id\":\"att4MPT0gu8r2DZdv\",\"width\":414,\"height\":526,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/CPUzMG77y3K0rA0tWU8o6w/pxh4dReWiftrJv9b1Eo86OOF7-_3RJoVp_L52AwuXAnN-YKeePsqpSRIIu7nTcXm/6NHW8i06htQyw_dGyllAg_JBesrIQB0wXcKaN8VAU7g\",\"filename\":\"Mother_and_child.jpg\",\"size\":38679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/5SWhqoMS2WsDHfm2Vobo2Q/pARCyYR-nkEhGsLYPv5lMXj9AKzmgA4aG6U0CB7oetcTy3WW4lRO086gVYClRm4d/i6tG378kTD402KuJc3OrhtBZbGXPo0X1Qup4v3HVRDA\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1o61CS4NAY95mVVFZkFAHQ/87ublhCOD7O_vw9_CorBWGLDhaIDfTIePp1huUxhSKyPRn4d3BhkOoChOE7I7Vc2/o2NLtrXqBvCdfyopdGDCv0HAVK6yIjVkuA3F8kfFKJA\",\"width\":414,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/3w03xXXAp-vAHrdQ1i18pw/DFYQN7JVMKn9VMS0MR5_LGHmTuUB104JN53svlkhuUNWrWwAPtvv7eE5aupLmBWa/IEHegAdxVxSkrL_FfHRfji4Vc4FcFxC6KuWY-E161Vg\",\"width\":414,\"height\":526}}},{\"id\":\"attsGNtljepdSppj3\",\"width\":3694,\"height\":2916,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/f8iRSVC3GILZXqUtQFrq4Q/cUdshOQNKZFUiYLfztB1F5YPwpBNdr5P2gChBUycvuT1DvfQXF5YFQBj3qykjTud/Ane0w5HrvGVD-6vmiygh3BNs6T9ovEsgO7eUQSc3f5A\",\"filename\":\"Sky-gate.jpg\",\"size\":10002210,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/QwEQlQixgOqkn8nslN2Y6g/IL83Rg0u9KpO679by57L4N5B9_V3usQXnSKHoZFRWQyr5RuepZIdHn09MdbDoRjf/JHL13nwJckwS4Zd7bVQiVpao6iVZpfsKXeaIKApWLWA\",\"width\":46,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/oKxweEQcMivtBwWJbokbPg/uc1tD7j9mfSvU2ZHf02ypEn129Auez_gJIompbI3vApbaeCX-NfjF82SP-JKhnFR/2sjkt0hOM6jzz-mOMVXllnlnQKDolzpLLn406-CTQJI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/kFallbj8w8BCSI_XC1Fqwg/TGbmFkXXU04v2xR6IPxu0KUOOrvdeLDtsXaAwa0iX7xoVbl7xKfqdx1MoWWC1ZSt/NxNxeFf-LUSSvdI5GywlF8IrKrtYDSgvYnPq1pn2Jyw\",\"width\":3000,\"height\":2368}}},{\"id\":\"attpYKQ4gldei2tWd\",\"width\":960,\"height\":696,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/SOSc7Wvam2A2Poh884R0KQ/QEvi6TyQ365GuWaR-vvCddbyE2SixWKu42TpQtm8pQN8CdEkJXnMwaMihOHT7Ria/PPJY4tf-bw7aByqzr3nxouFe7rjoEeMS-nPvTbFEJAU\",\"filename\":\"Akari_Lamps.jpg\",\"size\":110954,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/vdQhCL3Sf0o0WvqdfcvZGg/tMEhb2vxY-U_9RF7yukhGgVJhK50xvUn8VkCe0CNgEDDli4adUiZTa4fsXMK5FvJ/JAID86P8QZqVDza0tOLsOW4vNEw-wDLBygMefJWHcS8\",\"width\":50,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/YOypgJVIPhF3Ad6kRmyi1Q/tuaUGFPR7ibfhl6Ig8bkXxCmNhJvUIuW0ET7uvFYWFZteb97O67eWvj1DsQ8miea/6BptX9BHxZnpyyVm2AOrsW9UUiBGS7bmM01QPt09HzY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/vaqG64yXJeKq4nADSnVVLQ/1s8MWpB87kemh4xvb_b_hkMU3zTFCdIGgQSlswo8uLMBNkaljnGH-Z-CxOCdeM1O/Stgp6l-t3I4TjU1ZPfJbKJkDr_kJEwXPP3qJnLFvjmY\",\"width\":960,\"height\":696}}}]}},{\"id\":\"recneNPDcZsNQDxsb\",\"createdTime\":\"2015-02-09T23:28:08.000Z\",\"fields\":{\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Thornton Willis is an American abstract painter. He has contributed to the New York School of painting since the late 1960s. Viewed as a member of the Third Generation of American Abstract Expressionists, his work is associated with Abstract Expressionism, Lyrical Abstraction, Process Art, Postminimalism, Bio-morphic Cubism (a term he coined) and Color Field painting.\",\"Name\":\"Thornton Willis\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"att8enrlgYD3FiHXB\",\"width\":433,\"height\":550,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/yv9xHxuxZ2ZcHDEKj2YoRA/__ngiSxpnsbDKW0639KFeJMAfe37-koPYnEHk26iasS9WA2IDo3fbYi6O5a0bJ5U/hMp6KYwfuPRrMGfhoxoVAf2dF6Ag5sCl5dMQI2KnbeE\",\"filename\":\"Color-Drawing.jpg\",\"size\":374784,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/aFaLsjsQ9M7GkxQM6JeOJg/VRirewmoY1ytkpl7jzqug5XcuPMTxahnk5tPn7ksfkNS8az-8OZPlQK0cgatDMt-/izp7lonZxpzaO8uHWyLl02PhI1MRiIi2l6FOxvE0svo\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/aZPJwEOa_-QXV1IOabXoBw/LUxzuQr2QGUOSzACrtm-QGUAw6ME61ghY1stEm3JvP8MkNTPV6rPsjGFD1K0Vsyf/eKCskL1fnWuJy_sIdPPEcE4GpR9dRa-g39EEtEWylRc\",\"width\":433,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Te98kzvnZqayF3Gj_BKXfA/IgpJv8UUOm_PX3URF_-5Eu7ELpZZfIRLGq8rkA8sgcejINXGjT81pfNcZ3CU1FXA/aUR7uIm3IzN6XY_VcnVaMeHzNvnP9rzHDcL_4sGRY9M\",\"width\":433,\"height\":550}}},{\"id\":\"attqEXWllptvGjPDQ\",\"width\":551,\"height\":700,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/rxsDe1RXkC-IDB4E41lj5g/dV_yu4NtBSMZtYzNR32xkQkPgjXjDMkdRw9fgK-zumhtFcR1z7Y-mjQyw04u-x-P/lNyNVttQHn152X-oIrnIowxYxS7KuwxnKEnLU7u6oRQ\",\"filename\":\"study_No._2.jpg\",\"size\":109204,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qEQ5JjdXdEqYTFj0SbHPHw/cFUCF0eR0xuwMs79LlrnEf322o-dHFD77SqsbdIQqotGPZlDzOIOn293YMxSyOh5/OOps2hpx_gJpdC69RijszeHKMWc5n90YjJEwlCf-uw4\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/GYFSWwNlkfFgwVAVSSu8pA/i-OYWcoy6cLyowt3jydxKy18o0nLDZMvALqeJM1O0YfEMjLR0rxpva7D2pDWBnBl/EoZZa6rEp3Ku6yY7dbu8ajEa3TvYKpwNa2DPywT2gj8\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/geqcXR130X62oMcAxjIuWg/iryAeV3r-7EPD3pStNxNMEAVNARgxfAT7oKmeKddSfjJVPldqcrUlUlIS42DU3n1/xCrz2y7PpoScVRJej0MQ1U0_xMPuX11HUiwavV-_ECI\",\"width\":551,\"height\":700}}},{\"id\":\"attHI8lwWwEoooJ24\",\"width\":890,\"height\":1200,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/bN-qgfHWZkFHQaUaN4eWMw/NC7qrdMUku_hqlwFaAvnbMAk3UeuoRk9udT9Mfyfr1RudQqfXehOLqSO5Zc062EV/YBnwAZT7DVpmCMydHhiZNXSzuVJ2wDZlrHl7o6GbG-w\",\"filename\":\"three_gray_squares.jpg\",\"size\":109091,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/stVYYQ1ZCXLjoE146QipJQ/_Tw2m_2EoiOWwZpj46p3GRkzxJ3yyGUjBro8tdjaGR52d0SEo0rt0_MneMzSzIro-WdP21aF2gWJClG-ylJjQw/rehAZPUJkuNCk_EK_LenDV5kPC77At6koUe6Q1tilqw\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/00FcWOJjm0yDwiNHQYVxQg/mFkbIPUfcHZHOA_pX2wYJ4N2C6n9rOcXqKXoi4XOwlemA8ZIZRktLGjCp81kx-Tj-k6QgI_2WUnB3ITE5nQ4Kw/UVvJlRUDKW2zH4l-bJj3MUL-MRJYUFl8bOpoi6iCoZk\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/fhu4WwgF4OygVjPMdvbGxg/2ggM-4nQm2iFb9V-DCRfqN4XOoDshazFovsn0TWbbKiwOWZX3o5LYEwdyMAoj60X8pqBNn9TlV9MvruJwbj1UA/4yFiPoRh7L2PIbOwMnLwz-V7o27_A11LpZJbuhWl2zM\",\"width\":890,\"height\":1200}}},{\"id\":\"att5TIJ00ppiNYQm3\",\"width\":639,\"height\":517,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/AkOnl0EMMXDgWy2-1bfnAQ/7eARbqlAlg96LmJuHmqBl5zNU3F_4nS3aaK5xSmQQZK1wIltnAwewlkvgZPrQ8wi/5h7M8vJPtTkgp6NqZ_-3iMMiMvXpaindioIXv8ruoPk\",\"filename\":\"Streets_of_Tupelo.jpg\",\"size\":27664,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qp_pNqwe6u_DHnChkm9EEg/ILxIf4t7ayBIKWuKIpQ_MYg068JXkf705J3Q1Yr7oTADkwzB_Uj260wpSeqNTvtyy_btjS8aPs_9BR3Oz3bcyg/dCS0t3zl6mc_RslcTyCs-PXvWC4F4F25M25s1aaPPrk\",\"width\":44,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Z22BwlyRxNdMlrrH9MHtzQ/s9oqLStn-hp3AIOu6IZ0_BvSxIOcXGmSQb48vmis9DFcm8GkDYxXNNVKL_ryDeYDwN-Rekj8tdlcH1_YYavSQg/zVmykYWF6Gqp4XBo7guA1Lv_Ucu6P9k5BknXiFcYBZQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/VW8XROcPSs6u9YxL1cMHMw/Q_wIHpp7hjy4fWFt0WoVJpL1MIP_ual6UHSyByaO3DPtpakiAcyMFT5LXvcWDy9I/6bzQWlATSxTOLmhA13ZgZmkn9wiFPXGp-eAtffmchV0\",\"width\":639,\"height\":517}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/GoJsfjcQ-dgcFx1F2d2qUw/tj4FG5snjzxkrAEI27jPzHReh5nKUm7Z1z2k9k0n8bm_ul_LRg9dlj2mfcKRk4f5/i5SZKJhHR86GKvzy-W-9tgr9dnN-jfllmnWyH461Z2A\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/7qrj9d1-WI6_IqprUuL63w/-kmzYAaVG8boiUupuKRXUV2kZQssGd-MVsnNb4nuUAktAyxTCayh5oFrkTp649TJ/D5F9EAY42r6dW5GTmmbZG9WPb6lll_yco8cPpiNAvuM\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HanQcO5l5itrnEoJvOpM4g/sq4hz2Z3vGUxNh35QRqMlRyNqZiyAQ_ghGjruuWpmCuU5QdzD32EUT263SqOsS4s/4eIhoSgxoOqQX1I0Wq3_gTG69ukcJeyEu0GBE_uQouQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/c93vRr5teXAGgw8p3WgPaQ/vxq-24t-JVpe_ZKg25DgAWn6k_tIkufS_6_mFsUGC2PbwyOEIIirwNMvCZTZXhDD/rzILsKoDIrMeIUahol5vFXAURgXil_uBERTaGkcR-qw\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zlOnaEgEDiB86LJaowingw/fA5SyRQtzRHtSBEMNJW7kSwq8GBtv1taJ1EFxRoiqqKBuri9V9aL8_0yu1Lpka0A7Apr1rE8K1BLT0ybBDBwRQ/tZQdKaGcqkRm2w1fvD2hWUbybMlEgHeQc9K7n1cu8tQ\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Kgs_SvSqybXg4M1tZJm5cw/QkVeqSQRN-sorTaB18pPaTTS_c0UXcTlWHz0zAzUR1Il4rXuRCREh-eqsiHq6UYSLfIP0VpgZPC-bbzKPf_ozw/DPgkFtrHUGhdcNzV2L9TRekA2qZLN-9kS_gAP0fTUgA\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/OpWL9ql-qRhYocT5kfV92w/XgElInhWNE8WcGUjxeipYp0IbTp9E44Kk8PdV_gQr2K9Wf9jN2p979qw-gsoCYgvylIhcU4E0_t53UyAuKVj0g/V74yqGMKqAPZN9w6zVwP9MaAPmbx3Al-il8U6c0sVrM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/V4ytQHf8bCihnMsJ7fMyiw/a3PAN-omfhxWqGxPRctUEthLXmaSkY2ZuLFtK7vD4pvXFupQ7xmWr3CKvyNEQZ_tAnckHKxCls9SEuLhMWwtNw/-s5YiZfOw0uKwCWZ97jJ-Y_8ieB36dhC-hil8ZHivhU\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zk3ktaxvd7aRG1of8BbOtw/MU0udt_s_iFNkVkw2KJwm2wnFdigLqQcWB5J29dAt4Ot1hBRizcO30tTfXDHkTf6sRpHQ2652SYKm9gbUNmGSZbGSCIKZEc-HF2cAQoJWiVqV-3xA6U3iaQkXJgfYniznYaM7J5WoeHL3JmcybhAtQ/3lJBdKA4SIcY0p2QyUgYw_F7aiCuoUIlHfWIC6v6Zps\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UwK69wQ8ZpIIkp7S4mpgEQ/EhAKnXfUtEigaMZ1--HYltrQRW_XIa2ZYe9XpUCNulVfLvv8yIRnf25YfnxoK814BpX38ZC65RKYtc7xDikmYA9LoIgOKwlo9z_9yqLwy2R22tKd6JilIcitNlI2H81xrrA2Uwz2tYhNOEimk80UsQ/4iy0EuV8951kQpwM67yAD2wRmHG53dErnD1lP5yUuwE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/T1eMM58E2Up60PALN-hIqw/_oYbjD7N7jImNwIfrPapfQ2YYcOAQSoGnL38hIsgdJ1t-qSDEzkFlrS6C3Lrx_2l4OXw_VuvK8TeX1R1Fcb6bbEy-JlAJIH0vTrr03Ct3EfptXriBPPpODPqtx_bMPHW8s4mo6CxXZRd4waEX7_qPw/HJdWG45nQDMGu9kK8pPXaOSYqEcB1YcQDgqqF1f4dRU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/u_3HMusgY3FmD0ntOTTAYw/-4vgpJgLKpaaH3WoiAW-XkiD20FXlt7eEzSBrnioT6F0rXIdXv_fGV_Edi8Z7ATW5ZNb70id-ASy8X2aZfenLuG6fLAwiyXoSib1kGF295eDQeYpRqb1eraQB4gPESadDorC-b2-j2KVIv2hYgkgHA/6pAadbXGlqKznf_n3_mrhVz_FV3TBZupiMZoFGVcc-Y\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/v5RRKQ2rrnaT7BpG6YxUmw/zWxV8QqQyxLtGH-wlTKTGdZAHM3HRqzuZ74rxapRfEvBlnUwJVho3_Xpk2_1oW9h0IB36D5KbRm4CUxM-eIRgQFMpY388-FgwQaOU5KkA3ooUc7z4AEgZNIH6DDKyincqfmw877fxGS5d7KpfwM3UiaQg8dUa9vt-aGFggnZWwodrcK_yO-_vnKqKvLd3Kk9/YJHZ_QmdwR5c_VB5qGD9lfOhjC3-ESOSVLaohvrGonU\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/6P0256TFuxT-dEjega0mcg/VI2e2WYIq4gWXnNIB_pBp4OXy3LF_OzCJF99RRdXflVOej6p_zLq2R6L5KWsMDOAFhqrbTcFJ-yt1pUdVKD7txrKWbb4TBCVXTPiaI3WG-fuysulumFqjgkhYSjv9UtAJNmqrln4KGPSRhwGhBVkh0TnAcUdWdsDiqRSGG29ZTnUakc2s3fWWx7nkJaUOpec/4owxPLxFK8qOiXKWCPljACDN_bNTycfKd2bnpjdirPE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1fzFnpP0RTA24_QM1glSbQ/4IrmXRvQ4jf8_MprceWnFX94gHzt_9XTWrhvUEkfeOHY3vqxe-fDnSUjW24PPeQROu7Vef2pyk4ctHGd4_mMc3EqnjvmTrupq1b_Z2jE8riLiDyqtCp1NntCWZJJP5B5kNOVIEytsqt-OJ1RHCoARF-kE-AKfHhOWUQxGr9hrQYDfS_E0HO7A69AdP3Y0N4F/qJ84hKf-ML33Z5W8iWO0M8Mysuid-00M3hQPDICpCzg\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/JfTwylMJVY-QmcZkPVsfSA/FP7ATfIkL5SOAHa1Cq6DylMn9V4q8wwMMlwRBYM1_OE2Agn7fB6PtxS0V0aaCKszQMmCO3nU5ysu6S-m80PoI8K1ojuMiGWC2QSLI2rX2IJuzlOJBuCX8kp04rbVYjbxVuyWk18Cx7cQnnhYFuaYa-n4CVSbKrKCNqYI5CvQAYaRSE8bgmGpoKlMzrBt_KZZ/Jk4gxMYhLz5qQPs_hSNszIygOY8ef27xyk_UzmrbsD8\",\"width\":600,\"height\":600}}}]}}]}");

            string bodyText = "{\"view\":\"Gallery View\",\"returnFieldsByFieldId\":false}";
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse, bodyText);

            Task<ListAllRecordsTestResponse> task = ListAllRecords(view: "Gallery View");
            var response = await task;
            Assert.IsTrue(response.Success);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.ThAtApiClientSortTest
        // List records
        // Use the 'sort' parameter to specify how the records will be ordered.
        // If you set the view parameter, the returned records in that view will be sorted by criteria set in the sort objects.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task ThAtApiClientSortTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recneNPDcZsNQDxsb\",\"createdTime\":\"2015-02-09T23:28:08.000Z\",\"fields\":{\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Thornton Willis is an American abstract painter. He has contributed to the New York School of painting since the late 1960s. Viewed as a member of the Third Generation of American Abstract Expressionists, his work is associated with Abstract Expressionism, Lyrical Abstraction, Process Art, Postminimalism, Bio-morphic Cubism (a term he coined) and Color Field painting.\",\"Name\":\"Thornton Willis\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"att8enrlgYD3FiHXB\",\"width\":433,\"height\":550,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/yv9xHxuxZ2ZcHDEKj2YoRA/__ngiSxpnsbDKW0639KFeJMAfe37-koPYnEHk26iasS9WA2IDo3fbYi6O5a0bJ5U/hMp6KYwfuPRrMGfhoxoVAf2dF6Ag5sCl5dMQI2KnbeE\",\"filename\":\"Color-Drawing.jpg\",\"size\":374784,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/aFaLsjsQ9M7GkxQM6JeOJg/VRirewmoY1ytkpl7jzqug5XcuPMTxahnk5tPn7ksfkNS8az-8OZPlQK0cgatDMt-/izp7lonZxpzaO8uHWyLl02PhI1MRiIi2l6FOxvE0svo\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/aZPJwEOa_-QXV1IOabXoBw/LUxzuQr2QGUOSzACrtm-QGUAw6ME61ghY1stEm3JvP8MkNTPV6rPsjGFD1K0Vsyf/eKCskL1fnWuJy_sIdPPEcE4GpR9dRa-g39EEtEWylRc\",\"width\":433,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Te98kzvnZqayF3Gj_BKXfA/IgpJv8UUOm_PX3URF_-5Eu7ELpZZfIRLGq8rkA8sgcejINXGjT81pfNcZ3CU1FXA/aUR7uIm3IzN6XY_VcnVaMeHzNvnP9rzHDcL_4sGRY9M\",\"width\":433,\"height\":550}}},{\"id\":\"attqEXWllptvGjPDQ\",\"width\":551,\"height\":700,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/rxsDe1RXkC-IDB4E41lj5g/dV_yu4NtBSMZtYzNR32xkQkPgjXjDMkdRw9fgK-zumhtFcR1z7Y-mjQyw04u-x-P/lNyNVttQHn152X-oIrnIowxYxS7KuwxnKEnLU7u6oRQ\",\"filename\":\"study_No._2.jpg\",\"size\":109204,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qEQ5JjdXdEqYTFj0SbHPHw/cFUCF0eR0xuwMs79LlrnEf322o-dHFD77SqsbdIQqotGPZlDzOIOn293YMxSyOh5/OOps2hpx_gJpdC69RijszeHKMWc5n90YjJEwlCf-uw4\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/GYFSWwNlkfFgwVAVSSu8pA/i-OYWcoy6cLyowt3jydxKy18o0nLDZMvALqeJM1O0YfEMjLR0rxpva7D2pDWBnBl/EoZZa6rEp3Ku6yY7dbu8ajEa3TvYKpwNa2DPywT2gj8\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/geqcXR130X62oMcAxjIuWg/iryAeV3r-7EPD3pStNxNMEAVNARgxfAT7oKmeKddSfjJVPldqcrUlUlIS42DU3n1/xCrz2y7PpoScVRJej0MQ1U0_xMPuX11HUiwavV-_ECI\",\"width\":551,\"height\":700}}},{\"id\":\"attHI8lwWwEoooJ24\",\"width\":890,\"height\":1200,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/bN-qgfHWZkFHQaUaN4eWMw/NC7qrdMUku_hqlwFaAvnbMAk3UeuoRk9udT9Mfyfr1RudQqfXehOLqSO5Zc062EV/YBnwAZT7DVpmCMydHhiZNXSzuVJ2wDZlrHl7o6GbG-w\",\"filename\":\"three_gray_squares.jpg\",\"size\":109091,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/stVYYQ1ZCXLjoE146QipJQ/_Tw2m_2EoiOWwZpj46p3GRkzxJ3yyGUjBro8tdjaGR52d0SEo0rt0_MneMzSzIro-WdP21aF2gWJClG-ylJjQw/rehAZPUJkuNCk_EK_LenDV5kPC77At6koUe6Q1tilqw\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/00FcWOJjm0yDwiNHQYVxQg/mFkbIPUfcHZHOA_pX2wYJ4N2C6n9rOcXqKXoi4XOwlemA8ZIZRktLGjCp81kx-Tj-k6QgI_2WUnB3ITE5nQ4Kw/UVvJlRUDKW2zH4l-bJj3MUL-MRJYUFl8bOpoi6iCoZk\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/fhu4WwgF4OygVjPMdvbGxg/2ggM-4nQm2iFb9V-DCRfqN4XOoDshazFovsn0TWbbKiwOWZX3o5LYEwdyMAoj60X8pqBNn9TlV9MvruJwbj1UA/4yFiPoRh7L2PIbOwMnLwz-V7o27_A11LpZJbuhWl2zM\",\"width\":890,\"height\":1200}}},{\"id\":\"att5TIJ00ppiNYQm3\",\"width\":639,\"height\":517,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/AkOnl0EMMXDgWy2-1bfnAQ/7eARbqlAlg96LmJuHmqBl5zNU3F_4nS3aaK5xSmQQZK1wIltnAwewlkvgZPrQ8wi/5h7M8vJPtTkgp6NqZ_-3iMMiMvXpaindioIXv8ruoPk\",\"filename\":\"Streets_of_Tupelo.jpg\",\"size\":27664,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qp_pNqwe6u_DHnChkm9EEg/ILxIf4t7ayBIKWuKIpQ_MYg068JXkf705J3Q1Yr7oTADkwzB_Uj260wpSeqNTvtyy_btjS8aPs_9BR3Oz3bcyg/dCS0t3zl6mc_RslcTyCs-PXvWC4F4F25M25s1aaPPrk\",\"width\":44,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Z22BwlyRxNdMlrrH9MHtzQ/s9oqLStn-hp3AIOu6IZ0_BvSxIOcXGmSQb48vmis9DFcm8GkDYxXNNVKL_ryDeYDwN-Rekj8tdlcH1_YYavSQg/zVmykYWF6Gqp4XBo7guA1Lv_Ucu6P9k5BknXiFcYBZQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/VW8XROcPSs6u9YxL1cMHMw/Q_wIHpp7hjy4fWFt0WoVJpL1MIP_ual6UHSyByaO3DPtpakiAcyMFT5LXvcWDy9I/6bzQWlATSxTOLmhA13ZgZmkn9wiFPXGp-eAtffmchV0\",\"width\":639,\"height\":517}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/GoJsfjcQ-dgcFx1F2d2qUw/tj4FG5snjzxkrAEI27jPzHReh5nKUm7Z1z2k9k0n8bm_ul_LRg9dlj2mfcKRk4f5/i5SZKJhHR86GKvzy-W-9tgr9dnN-jfllmnWyH461Z2A\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/7qrj9d1-WI6_IqprUuL63w/-kmzYAaVG8boiUupuKRXUV2kZQssGd-MVsnNb4nuUAktAyxTCayh5oFrkTp649TJ/D5F9EAY42r6dW5GTmmbZG9WPb6lll_yco8cPpiNAvuM\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HanQcO5l5itrnEoJvOpM4g/sq4hz2Z3vGUxNh35QRqMlRyNqZiyAQ_ghGjruuWpmCuU5QdzD32EUT263SqOsS4s/4eIhoSgxoOqQX1I0Wq3_gTG69ukcJeyEu0GBE_uQouQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/c93vRr5teXAGgw8p3WgPaQ/vxq-24t-JVpe_ZKg25DgAWn6k_tIkufS_6_mFsUGC2PbwyOEIIirwNMvCZTZXhDD/rzILsKoDIrMeIUahol5vFXAURgXil_uBERTaGkcR-qw\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zlOnaEgEDiB86LJaowingw/fA5SyRQtzRHtSBEMNJW7kSwq8GBtv1taJ1EFxRoiqqKBuri9V9aL8_0yu1Lpka0A7Apr1rE8K1BLT0ybBDBwRQ/tZQdKaGcqkRm2w1fvD2hWUbybMlEgHeQc9K7n1cu8tQ\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Kgs_SvSqybXg4M1tZJm5cw/QkVeqSQRN-sorTaB18pPaTTS_c0UXcTlWHz0zAzUR1Il4rXuRCREh-eqsiHq6UYSLfIP0VpgZPC-bbzKPf_ozw/DPgkFtrHUGhdcNzV2L9TRekA2qZLN-9kS_gAP0fTUgA\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/OpWL9ql-qRhYocT5kfV92w/XgElInhWNE8WcGUjxeipYp0IbTp9E44Kk8PdV_gQr2K9Wf9jN2p979qw-gsoCYgvylIhcU4E0_t53UyAuKVj0g/V74yqGMKqAPZN9w6zVwP9MaAPmbx3Al-il8U6c0sVrM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/V4ytQHf8bCihnMsJ7fMyiw/a3PAN-omfhxWqGxPRctUEthLXmaSkY2ZuLFtK7vD4pvXFupQ7xmWr3CKvyNEQZ_tAnckHKxCls9SEuLhMWwtNw/-s5YiZfOw0uKwCWZ97jJ-Y_8ieB36dhC-hil8ZHivhU\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zk3ktaxvd7aRG1of8BbOtw/MU0udt_s_iFNkVkw2KJwm2wnFdigLqQcWB5J29dAt4Ot1hBRizcO30tTfXDHkTf6sRpHQ2652SYKm9gbUNmGSZbGSCIKZEc-HF2cAQoJWiVqV-3xA6U3iaQkXJgfYniznYaM7J5WoeHL3JmcybhAtQ/3lJBdKA4SIcY0p2QyUgYw_F7aiCuoUIlHfWIC6v6Zps\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UwK69wQ8ZpIIkp7S4mpgEQ/EhAKnXfUtEigaMZ1--HYltrQRW_XIa2ZYe9XpUCNulVfLvv8yIRnf25YfnxoK814BpX38ZC65RKYtc7xDikmYA9LoIgOKwlo9z_9yqLwy2R22tKd6JilIcitNlI2H81xrrA2Uwz2tYhNOEimk80UsQ/4iy0EuV8951kQpwM67yAD2wRmHG53dErnD1lP5yUuwE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/T1eMM58E2Up60PALN-hIqw/_oYbjD7N7jImNwIfrPapfQ2YYcOAQSoGnL38hIsgdJ1t-qSDEzkFlrS6C3Lrx_2l4OXw_VuvK8TeX1R1Fcb6bbEy-JlAJIH0vTrr03Ct3EfptXriBPPpODPqtx_bMPHW8s4mo6CxXZRd4waEX7_qPw/HJdWG45nQDMGu9kK8pPXaOSYqEcB1YcQDgqqF1f4dRU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/u_3HMusgY3FmD0ntOTTAYw/-4vgpJgLKpaaH3WoiAW-XkiD20FXlt7eEzSBrnioT6F0rXIdXv_fGV_Edi8Z7ATW5ZNb70id-ASy8X2aZfenLuG6fLAwiyXoSib1kGF295eDQeYpRqb1eraQB4gPESadDorC-b2-j2KVIv2hYgkgHA/6pAadbXGlqKznf_n3_mrhVz_FV3TBZupiMZoFGVcc-Y\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/v5RRKQ2rrnaT7BpG6YxUmw/zWxV8QqQyxLtGH-wlTKTGdZAHM3HRqzuZ74rxapRfEvBlnUwJVho3_Xpk2_1oW9h0IB36D5KbRm4CUxM-eIRgQFMpY388-FgwQaOU5KkA3ooUc7z4AEgZNIH6DDKyincqfmw877fxGS5d7KpfwM3UiaQg8dUa9vt-aGFggnZWwodrcK_yO-_vnKqKvLd3Kk9/YJHZ_QmdwR5c_VB5qGD9lfOhjC3-ESOSVLaohvrGonU\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/6P0256TFuxT-dEjega0mcg/VI2e2WYIq4gWXnNIB_pBp4OXy3LF_OzCJF99RRdXflVOej6p_zLq2R6L5KWsMDOAFhqrbTcFJ-yt1pUdVKD7txrKWbb4TBCVXTPiaI3WG-fuysulumFqjgkhYSjv9UtAJNmqrln4KGPSRhwGhBVkh0TnAcUdWdsDiqRSGG29ZTnUakc2s3fWWx7nkJaUOpec/4owxPLxFK8qOiXKWCPljACDN_bNTycfKd2bnpjdirPE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1fzFnpP0RTA24_QM1glSbQ/4IrmXRvQ4jf8_MprceWnFX94gHzt_9XTWrhvUEkfeOHY3vqxe-fDnSUjW24PPeQROu7Vef2pyk4ctHGd4_mMc3EqnjvmTrupq1b_Z2jE8riLiDyqtCp1NntCWZJJP5B5kNOVIEytsqt-OJ1RHCoARF-kE-AKfHhOWUQxGr9hrQYDfS_E0HO7A69AdP3Y0N4F/qJ84hKf-ML33Z5W8iWO0M8Mysuid-00M3hQPDICpCzg\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/JfTwylMJVY-QmcZkPVsfSA/FP7ATfIkL5SOAHa1Cq6DylMn9V4q8wwMMlwRBYM1_OE2Agn7fB6PtxS0V0aaCKszQMmCO3nU5ysu6S-m80PoI8K1ojuMiGWC2QSLI2rX2IJuzlOJBuCX8kp04rbVYjbxVuyWk18Cx7cQnnhYFuaYa-n4CVSbKrKCNqYI5CvQAYaRSE8bgmGpoKlMzrBt_KZZ/Jk4gxMYhLz5qQPs_hSNszIygOY8ef27xyk_UzmrbsD8\",\"width\":600,\"height\":600}}}]}},{\"id\":\"recraBPRF3m5Te5Hn\",\"createdTime\":\"2015-02-09T23:24:10.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Mark Rothko is generally identified as an Abstract Expressionist. With Jackson Pollock and Willem de Kooning, he is one of the most famous postwar American artists.\\\\\\\\\\n\",\"Name\":\"Mark Rothko\",\"Testing Date\":\"2020-07-06T17:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attONu0jXlWNlHOxh\",\"width\":213,\"height\":260,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/PbUV3GShksF7eS90_rrBJQ/jM8_jTwippopEtSruqAyKnanitl7tSTSyYPLJox7LnBkgdRMVAYbGbuNfk4cv2UD/GIugB_QeOYiKQlZhbceOh5Cy2UcO2sfDCT8hJO-CM68\",\"filename\":\"No._18_1951.jpg\",\"size\":7416,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/dnTWghS_CLbLPErUdpkYlw/FcQvNKRB0IUQfS_OUKiPhZiMK36nI5uh2gw-MRxAH0SiW4bnY4YjHa9dc_3F2hm8/yu8xDg3MnpBP47geWMbMApOBamBdu4GMLNisTHSw5KU\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Owm3s_T0ClZATmWYh8Yd3Q/KNsh-aNVWjL63-eAeeiKCjCNthx0OboZqnzNZTeZlIR2rG4djaZuoYI2rchAuU6i/gWGUHp15-bh2QVVTe3fU7-4pFv_69DbvA-RZnTypZ80\",\"width\":213,\"height\":260},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/QFT0oniW5fRt4uerd76_UA/dGGo5QWu_PJH0lmYBvOp3YLMuGZprv6uRwJckR64FB3F5sph6g-fpdJDWUWRJN_x/yuce_4yYLPycwODVjZjJrPoz1x7y1ERdwS4vVgmDyX4\",\"width\":213,\"height\":260}}},{\"id\":\"atteYo0fXP5bOpxt6\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/jy6-f9zfGfaQ6GfXh46Eag/8hAPj2fzKgkSkBaTc0tgHPY78yf9uf3KOOX-vimCysVIGEcyZisxIWl0BJlrL0Mi/40UHv833uyx9BnIMeBQbWQm8SHHKZVegG8kBp_OEph8\",\"filename\":\"Untitled_1954_RISD.jpg\",\"size\":23636,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/BfHoD3VhEG2sfKySKfYGpA/j1ihCP8vlaM-YLo4AYJROZrnm9uf6_IW7LAEWOjIBLzw_X7EGWbxDrsZEM6FrupX_YoCc1EXHEtBXr5l6jLh3Q/eQF8yaXoqMTV94o2to6uKCDYAYc0jglFKbMe-t05_sA\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/kM4zjv9Kg2rQ9R2uIKsBVA/vXY_-TIXn2v_8rc0EpPnJobnNejdiDyubBNa2a8FXcrMTChgY9r4IdyMqs--NkBrsVg5eKAgRR4MqCfHRti0kQ/FDuAxDUXBFjTuIFdIWx7bo21iBfmR6lZsZwSnxg90d0\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/tpxRiskTPIUHcFwamGG2lw/YhzPHu_kgmadbBr16ZSmvtnMainogo-e6qjRdyv-yQQ7if6hb5Qo44r0Cqhftwo9puhko96HE03tA3GMzOcHPA/GgkK4JDHGox8xsjx1tfyFM9_KO3yp8UAQGH7j1t1XJY\",\"width\":385,\"height\":640}}},{\"id\":\"attneJn9hR5DtDns9\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/ek8PNYj99sbtEYG546HJjQ/M4lTD-70Sp7ZW2ySxFOIr6mHG6QlZZK7476OZ900ABMW8YudhRz77ELOnt5jxS5i/0TdF7mfi6b1lTBRfLqHZtYbb7nEceEjsS3QmcYqEsiE\",\"filename\":\"Untitled_1954.jpg\",\"size\":33352,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/ksoZ22Zhvxj_GqWuij14Xg/dD-UyXwVh2gmBZlmX3fmHSCg7pFYBVsuxUi5KA_d4rmPKpab8NBdLsh8kn9FCmcQ/J05MFpNIYBRnsBksyWTr0yyk3LgMg9VhniAmRVfHUY0\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/QP6YIS7IA4qxDllTCBJJXA/IV_nNgQjbI8-5kNPTnNCbq4JZZqsCh2YLnmiXFK81W_Csqn48n1nZNLScRKAbMDy/JtvnBui_46sAsKMJcl06QIk3ChiEhZvjSwY3UaDKbGo\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/70hsT0xUHiHo1TqyspxZAA/FVvlgrYmT3-TL8LxkYVhAOH5EqNjgwLaD3R7NYOb6IW0ijZyl-MqxBFEdkExS1TW/TnwzNpwwlH-0phY_bJbVsNqCV1MTN_USzHtM8044ef0\",\"width\":385,\"height\":640}}},{\"id\":\"attnicjT3NIYNL5Le\",\"width\":551,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RCSRZQDTEcw0hSma0CTf9Q/5J0xZocW3v2EJmAqFMysSTDwAgwLd4pGLmuFlhb2VyQy3v2YxdCj2bl8IoT5iNEq/aa7M3e4EXDJhVSMyouhlY9BfTy4zV_T6P1WhBXsrExw\",\"filename\":\"Untitled_(Red,_Orange).jpg\",\"size\":43346,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/MRK_9Zfb1-QoyAW7_fYjag/z5o01IpdLlRHExEp0-BV_YWiswTqh75ql2Y9PC98c-LIepXOVaEEWyU2MbkZxaEKkac62ZhQoHpuynIxAlvTig/OkE3ZIBBKAvMMtOqai_LEbXoHfPgvpMHjUBT6d2qhnE\",\"width\":31,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Nrk9zEjCb--X3e-CaDzHWQ/yVIFV-bnqEeAoiFTd8KC4UQdQxEBch7NUaRo5MrbxTSRoGtE99jlayyntii4qrjLjUMpmOFjGlTMBIuRxIF6wg/KhyOEj1x94gBLsswCwdfdtV80ylMdKtVKKcXODlO2TU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/KR_OHMXJgb2LTPg2FE0nqA/ZMjgjzDe6d1k5TTtkx1vzIk8QkLqriKh8l0uHYVGcc3vRcWUrynaJdVE0IM4PMf0UHoF52ImAUiE0DbxYs46CQ/Ms8AceaI5PADU3NdQ76qKZ7FROBEOHNDFNFVUKEKAxk\",\"width\":551,\"height\":640}}},{\"id\":\"attpDkpjaf734NjM0\",\"width\":480,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/4EliRVvEbjBXI_HG8UKJYg/2HOaXvOKkXb-VlV7xjDFmvb1ecqnEowy2TjwgBV37Vnr-hkVV-cl1-VuUfENaZFn/Oq5LoXCt_Lew6HzFqpzpxybNXiyDlLTicbu_qJudK9U\",\"filename\":\"No._61_(Rust_and_Blue).jpg\",\"size\":35819,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/F4W1skt82SPkKT6TYTj8fA/4T_w88TdVQoURMCX4kIwrMiwSOPgziQ2d_x53iJMJzV_xw6cHHagvHACZ0cn4C5hmupL0b8TXcV6CGt8BhS8IA/z_DKt7vrWDAG3Dc7pQxRtm2ZMaX-frUt-ne8J8AzPDc\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qhM_BCvbZIOd9QxppXnevw/0G_3GP_unGeDXRX4zu5kUfaMRYsyYMjBeeGg0WDxn9Tg1xRiieH9mlgdwcm5oT1Cxp4tXgx9RF0dCBF4Eky61A/4R7uYoLEKjUxZyUVvsBAZGTIf_oiUn2xEcDwL47Org0\",\"width\":480,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/aczP_KWUlOjTx4oWXPFxRw/CgTAwAwDUtklKVLb2Qb69IvgWN_vI1ZrVUUMCpVkGogpAwymvOKWT8uR9QMD84XeajX9AoEKNroA5I4PIgWdfA/xNOe9bfOoAimon4Y579IQf8C64Qa_SIMWjCBFVKzw2k\",\"width\":480,\"height\":600}}}]}},{\"id\":\"recj31Rc5TXAiVZV3\",\"createdTime\":\"2015-02-09T23:36:53.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Experimental Sculpture\"],\"Bio\":\"Isamu Noguchi (野口 勇) was a prominent Japanese American artist and landscape architect whose artistic career spanned six decades, from the 1920s onward. Known for his sculpture and public works, Noguchi also designed stage sets for various Martha Graham productions, and several mass-produced lamps and furniture pieces, some of which are still manufactured and sold.\",\"Name\":\"Isamu Noguchi\",\"Collection\":[\"reccV1ddwIspBOe4O\"],\"Attachments\":[{\"id\":\"attuoGtQSGoeWEurX\",\"width\":640,\"height\":487,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/yZTgKAaWcwfCExKnoEB1tQ/HoJrgwi6NUTDRp2qLH770PeeL3XIa4I5USmX9WR9hWc/rvhVZFT7v0RIwBMqsn2rvmpqh-4SUmcmoPbFkjXJreM\",\"filename\":\"Leda.jpeg\",\"size\":55738,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Xm984HlTOt-Evm7XypPheQ/M_yOhO_xPhMQ7vqJeeocbIBuy6hQqaTDSgD2Ttf9Ajbnc-mlAR9ESq1S0YMHYua5/lN29NGXCkjvoWJ6rTPtLXS7ujrqZW7oYtJOXhZCE6tE\",\"width\":47,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/TooUBGJwwGtHiAloT48u2g/jmA5DW0O9er_mBrNlus0Ehly2PZk5Zpd-7sp5b9GyfZCLaD-udl7ZJTCY-hwLAjm/TreykLTJP_5xSE_OH_sa8uwX9V0wOj8fQNt_77lMj9A\",\"width\":512,\"height\":487},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NWGeILM__eyRrhsYgG3KQg/9AP9mOkSq1zGiJOXvZSV_hvdVgoSFKIKbx3KPLcDolxqIHie8y3W2eTl6tLE9gPw/xSyZztnKIrLt82M1DGH6r-V7RpbFnVjTgn715Fit9r8\",\"width\":640,\"height\":487}}},{\"id\":\"att4MPT0gu8r2DZdv\",\"width\":414,\"height\":526,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/CPUzMG77y3K0rA0tWU8o6w/pxh4dReWiftrJv9b1Eo86OOF7-_3RJoVp_L52AwuXAnN-YKeePsqpSRIIu7nTcXm/6NHW8i06htQyw_dGyllAg_JBesrIQB0wXcKaN8VAU7g\",\"filename\":\"Mother_and_child.jpg\",\"size\":38679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/5SWhqoMS2WsDHfm2Vobo2Q/pARCyYR-nkEhGsLYPv5lMXj9AKzmgA4aG6U0CB7oetcTy3WW4lRO086gVYClRm4d/i6tG378kTD402KuJc3OrhtBZbGXPo0X1Qup4v3HVRDA\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1o61CS4NAY95mVVFZkFAHQ/87ublhCOD7O_vw9_CorBWGLDhaIDfTIePp1huUxhSKyPRn4d3BhkOoChOE7I7Vc2/o2NLtrXqBvCdfyopdGDCv0HAVK6yIjVkuA3F8kfFKJA\",\"width\":414,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/3w03xXXAp-vAHrdQ1i18pw/DFYQN7JVMKn9VMS0MR5_LGHmTuUB104JN53svlkhuUNWrWwAPtvv7eE5aupLmBWa/IEHegAdxVxSkrL_FfHRfji4Vc4FcFxC6KuWY-E161Vg\",\"width\":414,\"height\":526}}},{\"id\":\"attsGNtljepdSppj3\",\"width\":3694,\"height\":2916,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/f8iRSVC3GILZXqUtQFrq4Q/cUdshOQNKZFUiYLfztB1F5YPwpBNdr5P2gChBUycvuT1DvfQXF5YFQBj3qykjTud/Ane0w5HrvGVD-6vmiygh3BNs6T9ovEsgO7eUQSc3f5A\",\"filename\":\"Sky-gate.jpg\",\"size\":10002210,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/QwEQlQixgOqkn8nslN2Y6g/IL83Rg0u9KpO679by57L4N5B9_V3usQXnSKHoZFRWQyr5RuepZIdHn09MdbDoRjf/JHL13nwJckwS4Zd7bVQiVpao6iVZpfsKXeaIKApWLWA\",\"width\":46,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/oKxweEQcMivtBwWJbokbPg/uc1tD7j9mfSvU2ZHf02ypEn129Auez_gJIompbI3vApbaeCX-NfjF82SP-JKhnFR/2sjkt0hOM6jzz-mOMVXllnlnQKDolzpLLn406-CTQJI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/kFallbj8w8BCSI_XC1Fqwg/TGbmFkXXU04v2xR6IPxu0KUOOrvdeLDtsXaAwa0iX7xoVbl7xKfqdx1MoWWC1ZSt/NxNxeFf-LUSSvdI5GywlF8IrKrtYDSgvYnPq1pn2Jyw\",\"width\":3000,\"height\":2368}}},{\"id\":\"attpYKQ4gldei2tWd\",\"width\":960,\"height\":696,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/SOSc7Wvam2A2Poh884R0KQ/QEvi6TyQ365GuWaR-vvCddbyE2SixWKu42TpQtm8pQN8CdEkJXnMwaMihOHT7Ria/PPJY4tf-bw7aByqzr3nxouFe7rjoEeMS-nPvTbFEJAU\",\"filename\":\"Akari_Lamps.jpg\",\"size\":110954,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/vdQhCL3Sf0o0WvqdfcvZGg/tMEhb2vxY-U_9RF7yukhGgVJhK50xvUn8VkCe0CNgEDDli4adUiZTa4fsXMK5FvJ/JAID86P8QZqVDza0tOLsOW4vNEw-wDLBygMefJWHcS8\",\"width\":50,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/YOypgJVIPhF3Ad6kRmyi1Q/tuaUGFPR7ibfhl6Ig8bkXxCmNhJvUIuW0ET7uvFYWFZteb97O67eWvj1DsQ8miea/6BptX9BHxZnpyyVm2AOrsW9UUiBGS7bmM01QPt09HzY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/vaqG64yXJeKq4nADSnVVLQ/1s8MWpB87kemh4xvb_b_hkMU3zTFCdIGgQSlswo8uLMBNkaljnGH-Z-CxOCdeM1O/Stgp6l-t3I4TjU1ZPfJbKJkDr_kJEwXPP3qJnLFvjmY\",\"width\":960,\"height\":696}}}]}},{\"id\":\"recaaJrI2JbRgEX5O\",\"createdTime\":\"2015-02-10T00:15:45.000Z\",\"fields\":{\"Genre\":[\"Abstract Expressionism\",\"Modern art\",\"Surrealism\"],\"Bio\":\"Edvard Munch was a Norwegian painter and printmaker whose intensely evocative treatment of psychological themes built upon some of the main tenets of late 19th-century Symbolism and greatly influenced German Expressionism in the early 20th century. One of his most well-known works is The Scream of 1893.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Edvard Munch\",\"Testing Date\":\"2012-02-01T23:36:00.000Z\",\"Collection\":[\"recwpd7MLPQqorfcj\"],\"Attachments\":[{\"id\":\"attNIEYhExe4q53lp\",\"width\":1000,\"height\":579,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/bFflpUQdYiwyDyiknmqmAw/V8gThhMdkvKXhgrxXwa5fenZltQfwdTKHvWFzXcg7hDsYPIAZ9MRyYY5KakPeGkc/b-lQZtSkZ_tdDTVooD7b-EFqmcTvnfv1ywvD-96QrsI\",\"filename\":\"The_Sun.jpg\",\"size\":194051,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NF2YFUuzfI3T5NU-AcMhcA/rJyRsmGKbFZij8DmTigCfXaQqG3SvPji6XfGE5Gpaxi4l52S5MdWziFvAcbo2heI/i7zKE2mzUYtPW51Ww9Wiplc_7DZoQxiTCdJzM3FZH1Y\",\"width\":62,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/6Ce9TyYtcPdmc06XWEAlyQ/h0OucuxEeJwvxSEG_ljmEeTXvOtplcNmeQXlj-0VqUIZlwg_V6k11FLdh45Uu33v/KPy1MsN63B4nfsCClkaxNvcfq_OusegI-QhOWcjnj2k\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/_i-rVtbxkRNWU5P9qSrzIw/dHGCZEx_WouJiGP-ZAyRmtkIbKAUd6M-Fa2eEjM8fEFX-Jo_taHZUIKzPUyT3b_O/E1PCSOQ5xBKJhgf9nkAnL_IrtPs9I3lKM4vhXYliy5c\",\"width\":1000,\"height\":579}}},{\"id\":\"attVjzN5X8xdWoc2W\",\"width\":1458,\"height\":1500,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/jRPv-_oVfIfcuJYnxwFYQg/mvnfJlEXCKfxXeZgRNNAf5ON4FlZJngPAhRzk6Ce5LvQ0X9VWaU5RjW6tguJPDUhjQ7qj4eZrINz1lFUxpZdIg/r00FWA-x349oikEXLw4fcufgZtpeRMN2x8haynn7Knc\",\"filename\":\"Munch_Det_Syke_Barn_1896.jpg\",\"size\":425603,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/qTzsaQ3k10VyLOIYDk5_lA/M10OEbYi9Z6TungP8bHnXnOF8Ulhlsvgin_DmKIFasmoSv14RKwF_DD5RO0vK8GyK_HliPOh5lQTny5FRmbRXg/XY9Sfs9QAp4uz1xuhVabtZyaqwaVOFBAHu1STaZVlPQ\",\"width\":35,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/v6IGncr9VIzFytlfYaT7hw/JTzafGYZGr5-sG2W6SBkQpN9UFNaUCZj7wwxjotoCnKaFj4fF5mv78ECoF7O7QKvq-Y8OXDX_FwTIvNB1JSbkQ/_ZFXDJ3HnVjUxnYv_rFaGV6Oj1aZz5ppD--cdMClw_U\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/k1Am0kz48sPl0i0aVIswsw/lBN75X8Ip2ocW7V6_LBy7eIhPdwJnJzbHwi-CFfmrB4o7c5SSgIuzqA3f-zk4srv0-rsEnRncLcP9DlJ-NDJmw/StVJSyZNmrYkWub9yTPy6TTyAiecdL7rs4MCGY_doW8\",\"width\":1458,\"height\":1500}}},{\"id\":\"attnTOZBfCiHQhfy1\",\"width\":850,\"height\":765,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/jw-bgkiH2-9Wy3pqJWOUVw/zVaw-zAHE6ktzVU6FZ_JDW0mnyhG7QMMgdt7W38snT392JoC03A08q_VQ-CKZNMp/J3_DYSuKEOHgVOHV5v5NQxJD25OX6UVAeHRyVve62Gs\",\"filename\":\"death-in-the-sickroom.jpg\",\"size\":255101,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/vjWLTosrwyrEYNZtmfnjqQ/wKGNBcBvY2ZXxSoHUn_sL9xgqjKYmbbWu-MnitlKxtzURMHWsp1tm0g0c86rNta1MUWk0XEL60N6OX2o7Y9z_Q/c5cQcScaYLD9Kzn5vIhLWCE3jGWwiWsAbGv5pZKTvBo\",\"width\":40,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/G3hVzPL1D28T2DCVC9F1JA/WDKtHPTaTMV0zPYvwjX7kX74ucpMLAdLS4pkgIifMMWdImYwO_oFe_-KJKj1_mU-9KZEobGi3xax-K3afHgu9A/6OlpctkXtiAQeaFNyoww1aEof9SgOu6t9HpgkuSsBjY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zRCpnxCbMzoDT5ZzLGV4NA/zLMQzteVhQ6AujhCuyu-ew6T82oiXD2EiCjxjMQbe36A92haxZ8Q8UL53qIe0yuNuvKwXXtNN9WLH_Ty2Qeh2A/801oFPPuH2dHT4P_jvybpkczLlZM1ybTgZ1Sk9-9zOY\",\"width\":850,\"height\":765}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\\\\\\\\\n\\\\\\\\\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/dyGthKoDMnKwSI2KTA6wVA/UMVA2dQ5RSnfjfNlvN2--fzHihEvEsSBIjd9e5DH9TmNCyrrPgnyibB_aRuxpQnw/7g23WoYII1RjnZpHinLSkSfw-Dx_2zHuCTjpYiFm8ic\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RhEEH9XTSra1BtNxqAnztg/i7j3E9P8ZZs_CIJWr4ZHS6JHHgL--aPY9oiuf2Vee7BON7AQ4N-YXpXpbEIo4rxe/47rBsXKdVYGcTTgVnu3FXtnmZ0DyF8OXnpzmvPIsAfI\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/df87qbzGy6wJIanNCyBtrg/xUy28IShaE4kuXQ1qv9VT_xi8NMLRJNHGnowu7-axpnXr8mdoKjAFsLAvSRfL3UU/QqaimoVqaY-uqYPEac5fzhpkDZfOFZ6RZg0qIEderAA\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/rVIShNrp_34ji6XuooCMeQ/NnyruykjDN49ZGydPKfhe3CCVFdvyXZFYdXG9pqwGHET4AOGuKO_zYveGxZHzpz3/WEw11im0irKutvWzzuY1QroU95RzCr4RuwyksPha854\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/5oVv6rSzFGnN19MzVCL7pg/6_qT7KCq4jztukRdwBG17FAZFWBqkb_0pGwAC1nsn3bORCFwNXYjrWTzrLVP7_RQHwj98_FOgIyv414dylnmdQ/4F1nzs-VWwB5nW_zYG8eCGjw-JXuNWCqnp9fo_WozfU\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/nVkcUIfjWGvko5xzvtB_Gw/Vl_T7OaOV36M267RTg3p_ymwc5FMzj6vySE4MXkDtkRJjTWnkDjVvrleKNoQIJ7YH6Lhswqvp0LK8s8TW0CAug/hWdhfxnxEJ-JpWXibAArUE3ZoKHaiSas2XI-J-YNRz4\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/J9LtV7PbhIe_l-mgC8whVw/kwSbJnyHqWwdWc7GY712d3SgS-BKiPOh9ChBYCd_XXZaaUMc44991xsS487QW84W4E8zZMx9hmJuzVPnYrWywg/uhm_Q7EEB6UxIE-aRVp0yaRRgX5b3Qswfv0t8JZBjGo\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/8gY54C14V6hkctOxYFi9jw/rDl82T8e8XRjQGdV46JM5oBCFLpzKYDB4I4YQhbRilLPsPtRfvo-rkuHmOvz5VoJ2Rksj1JSur4Xr94nCc6_bw/oYGJpcqKaG8XZmD5ictdRsVr-tKixhBajfDgTjdMwzI\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HupXpyJP0RMjjfULl3JRrQ/eZg5CfzE-d7EBZmRUDIhv6t3-bfqWWllXlYeufGfTxpwTH44c5dD-hZGvKYBHgNA/AxWJsigWpfxcjPtgHkAP-KVO1SkOAjP1YiI81iTRF3A\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/NvQxlM9X5NQW7zOp4AOkTw/jW08CtxJQoA_rq2EvQOstuFPe-XpvikDGRhnjCFow61fqcYFft3CDz1Z2PR1BPgcNGfUqLmTjg-Yf7ZWXUtRYQ/H-sIC8zzV7Bxdf7LwYKc47zRL7n4gcAyncbygJskoAY\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1WwjreUt4BcwqqGu1_jlMQ/t3kmPFVWdlgI0zO4cyzm_5hnhAcfzJT-2p6LOHWZLd14shFRhoAfuZIAb3_UCo5ZQyrxCmHFFdkIzOrgK_Hw-w/I5C5EcuW1o2qzBGBQAuseP9XerIshRbFpsFn1xG-qHs\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Zg0G5_M_jwKtmw11dPPCHg/HEsRVXchLl8s0X9WtwmUAELkhTOL2mYbHTuCsqlwHpvSggYzqG--je8Kaqix-rH4D8uOzhFIYrCJbdOlCd349w/oaKrXWTLApH2eu4Spay1lOvIcfUCMgyeBrTs_xozCxw\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/39oBWza7hOYJJgaVt_yRpw/gaJUvLehOOfXILepGr7JiefPNcFMCedzVWknVqxiiRvHI9zqiWWbDW2OeIy7bnVj/OTmb3wZFcrc_MX-93zh_amU8azw1ROc9Mi-AUFvoWQ0\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/f4CQhJC03yufCMOak31dbQ/vwF_STC9EgXFsUkzCY-KG17kQnz8a-iDmBuKaaD3n-8pzXBJAvtDpnczqkuGtVTpYZcV3lYGLY3iX4qaiXnJqA/RlDEyWen99f4JeSuYurxTSFKZNlDd4v5tjYSoSpG2vQ\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/g48Aovs5yGIf2ZgLZwH2BQ/eyoJRrpw3IE2DxYX-_RP9DerRvEnFhPVU-Snqb684bwRb6qmRCguRRcBmszs9Es60AcrRCj8d9vR3ps7_xRSPA/EG_wDl7vmfQdm91Lh_N7IM0B87G-nLgkcsuALExevps\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/20mlEBWc6Bovof2ASIX1Zg/coJPJ4VPg24Q21fcIBI7SK1c9KZYW3zhSBDNORjrGgmDC_smPFerLHOVZOcBOUCPIizTpO9RRgdjf0VqUuJAXw/syA1G5xMrsCLEyR4VHnGss7n9sIqx-gjWPSOco5QFHs\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/I3-tYuTmDu89tn8_683myg/VnVWv9cniMd-24LQzZbNCbv8Y8LEdNcx0ngI6eHvH-yTE-zUJ8OWCBARH3-mtjJL/e-vhSIglR9avANBAL2rg-uOblPywKQ84TRVDNwiEGT0\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1Zxxi0BDpYmsBKka6hWATQ/C7nmfnT2nHHiFxQZEGa48v7bYDWdN0ISp5M2YT7klzBT3UvUjhGvpgvDRKBBRtm6/DHlRiTDu3Y8XtcAB0tUdvM_ROp-5W-yYNmGBxpEj7SE\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/TkpiadKNPKchJFC0nozMUw/7D0P5PjcQiHxt-R2gcV4ECwDwePR5t_3pQ-3V8Ieb0bbQEAkeKE4eHXqoawmqk7p/0Nj5NKoqhJtCHGiKI6Bapn_mUzI19LsLZnbKXEgDXz4\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/mAcRerRZUjZgNN0MMIZK7w/Drvhmq1inhnqUJaTTxiOhedWpjRHksluhv88-GXNm22BO37fLmbQszVSyyCL1syw/zNh7DmoMdPdDON8BgfQRSfabQf2XrOrjF1IRzn2xE4I\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/LJLILvRGC_OOu1kzFeclfA/8o3klF3vxIzq1MBJH2DH8RGvQTTpj9OGTdClHm78bgh4T7Xr_MbFAq0EGwo4k9sh/5j_h5ZCEmwPMSvEcdXRSLOHMaMdIM2DkpF13sfOAVog\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/tKwV1-9jqI1NfAlzp1mC6A/Kdnz2ID7WL5LaxKvLtY5mUG8NWZ-ypzsroTjiMvi88biBOPrCgfoVLBRGjsi0UiJ/AKXvKP63YcFBV0v5d5Lszmx7So5Nnw0SxpAFN1QYe74\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/P_XjuIaj3FZPiDALCLc_0A/6C4ktS8WKbnlSA1pmxKJV5yJS_AjoVAqhgSz2u7kSduwFGgY6B41WFoOgw58DaH0/36wu4pZXdqujq3GBu9tHwzQ4v0lqSnMry9hibfmWEnQ\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UrKyE50u0_m3DXznTljomQ/IqZyGe0im_rdzvvw8r4RkeiClqKrSARXR3CrebsI5lo4RnQ9HiN6hJyaFYM6J6lU/eTYwF5IhXhqqqdORJVEUKvObU-g3OIGWB_FbYXP7it4\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/AFMISdEguC43D-e2t6BRhw/id18VPoJ9MoFmTl-cwVkq7F9ifIfstM_wwhkkTaT4Ce0OSa2blTBzj1Ubcco0CeL/GJ4aC28m1h-QBvdhPtjZpr9kM_2AqxtvliU0ehwFiF8\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/r_vFrkyQ64TZJHLTmPAmlQ/VSDxC6OwSu38B_p40dStq5CaLkPZgdKmD7rMPHbco9e8ErnVIWe767wNT8hHkThl/YomqoVD18mfLgT3E8rCVHy64gaXWBkZxJzeJeGFKuA4\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/fEaI7AaMNNosIfAR3MI3Zw/NIS5mLHKpmD-3NAVhQhEZH8uRV6CT43bn4ndngoc5r63X8FNcE1AQ_YqFyKvcA17/98TzCdD66Ug2lnWLmjQupsqJFWlrfMJo9nyAIJ3FUOU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/RVdlXPRU-sF92yAU8zHM6A/vkpdS8zx_NvRBSDlxSNxu5-nBiSmKbz3r0hj9-nGXybutuAeFfm0oY6x188TW6EQ/s8e1sWCvynjZ9x9MXBe5mnQUfWaBRDdMkVGko6yywoQ\",\"width\":716,\"height\":720}}}]}}]}");

            string bodyText = "{\"sort\":[{\"field\":\"Name\",\"direction\":\"desc\"},{\"field\":\"Genre\",\"direction\":\"asc\"}],\"returnFieldsByFieldId\":false}";
            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/listRecords",
                    HttpMethod.Post,
                    fakeResponse,
                    bodyText);

            Sort sort1 = new Sort { Field = "Name", Direction = SortDirection.Desc};
            Sort sort2 = new Sort { Field = "Genre", Direction = SortDirection.Asc};
            List<Sort> sortList = new List<Sort>();
            sortList.Add(sort1);
            sortList.Add(sort2);

            Task<ListAllRecordsTestResponse> task = ListAllRecords(sort: sortList);
            var response = await task;
            Assert.IsTrue(response.Success);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TiAtApiClientRetrieveRecordTest
        //  Retrieve a record with a known record ID
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TiAtApiClientRetrieveRecordTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/GoJsfjcQ-dgcFx1F2d2qUw/tj4FG5snjzxkrAEI27jPzHReh5nKUm7Z1z2k9k0n8bm_ul_LRg9dlj2mfcKRk4f5/i5SZKJhHR86GKvzy-W-9tgr9dnN-jfllmnWyH461Z2A\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/7qrj9d1-WI6_IqprUuL63w/-kmzYAaVG8boiUupuKRXUV2kZQssGd-MVsnNb4nuUAktAyxTCayh5oFrkTp649TJ/D5F9EAY42r6dW5GTmmbZG9WPb6lll_yco8cPpiNAvuM\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HanQcO5l5itrnEoJvOpM4g/sq4hz2Z3vGUxNh35QRqMlRyNqZiyAQ_ghGjruuWpmCuU5QdzD32EUT263SqOsS4s/4eIhoSgxoOqQX1I0Wq3_gTG69ukcJeyEu0GBE_uQouQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/c93vRr5teXAGgw8p3WgPaQ/vxq-24t-JVpe_ZKg25DgAWn6k_tIkufS_6_mFsUGC2PbwyOEIIirwNMvCZTZXhDD/rzILsKoDIrMeIUahol5vFXAURgXil_uBERTaGkcR-qw\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zlOnaEgEDiB86LJaowingw/fA5SyRQtzRHtSBEMNJW7kSwq8GBtv1taJ1EFxRoiqqKBuri9V9aL8_0yu1Lpka0A7Apr1rE8K1BLT0ybBDBwRQ/tZQdKaGcqkRm2w1fvD2hWUbybMlEgHeQc9K7n1cu8tQ\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Kgs_SvSqybXg4M1tZJm5cw/QkVeqSQRN-sorTaB18pPaTTS_c0UXcTlWHz0zAzUR1Il4rXuRCREh-eqsiHq6UYSLfIP0VpgZPC-bbzKPf_ozw/DPgkFtrHUGhdcNzV2L9TRekA2qZLN-9kS_gAP0fTUgA\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/OpWL9ql-qRhYocT5kfV92w/XgElInhWNE8WcGUjxeipYp0IbTp9E44Kk8PdV_gQr2K9Wf9jN2p979qw-gsoCYgvylIhcU4E0_t53UyAuKVj0g/V74yqGMKqAPZN9w6zVwP9MaAPmbx3Al-il8U6c0sVrM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/V4ytQHf8bCihnMsJ7fMyiw/a3PAN-omfhxWqGxPRctUEthLXmaSkY2ZuLFtK7vD4pvXFupQ7xmWr3CKvyNEQZ_tAnckHKxCls9SEuLhMWwtNw/-s5YiZfOw0uKwCWZ97jJ-Y_8ieB36dhC-hil8ZHivhU\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zk3ktaxvd7aRG1of8BbOtw/MU0udt_s_iFNkVkw2KJwm2wnFdigLqQcWB5J29dAt4Ot1hBRizcO30tTfXDHkTf6sRpHQ2652SYKm9gbUNmGSZbGSCIKZEc-HF2cAQoJWiVqV-3xA6U3iaQkXJgfYniznYaM7J5WoeHL3JmcybhAtQ/3lJBdKA4SIcY0p2QyUgYw_F7aiCuoUIlHfWIC6v6Zps\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UwK69wQ8ZpIIkp7S4mpgEQ/EhAKnXfUtEigaMZ1--HYltrQRW_XIa2ZYe9XpUCNulVfLvv8yIRnf25YfnxoK814BpX38ZC65RKYtc7xDikmYA9LoIgOKwlo9z_9yqLwy2R22tKd6JilIcitNlI2H81xrrA2Uwz2tYhNOEimk80UsQ/4iy0EuV8951kQpwM67yAD2wRmHG53dErnD1lP5yUuwE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/T1eMM58E2Up60PALN-hIqw/_oYbjD7N7jImNwIfrPapfQ2YYcOAQSoGnL38hIsgdJ1t-qSDEzkFlrS6C3Lrx_2l4OXw_VuvK8TeX1R1Fcb6bbEy-JlAJIH0vTrr03Ct3EfptXriBPPpODPqtx_bMPHW8s4mo6CxXZRd4waEX7_qPw/HJdWG45nQDMGu9kK8pPXaOSYqEcB1YcQDgqqF1f4dRU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/u_3HMusgY3FmD0ntOTTAYw/-4vgpJgLKpaaH3WoiAW-XkiD20FXlt7eEzSBrnioT6F0rXIdXv_fGV_Edi8Z7ATW5ZNb70id-ASy8X2aZfenLuG6fLAwiyXoSib1kGF295eDQeYpRqb1eraQB4gPESadDorC-b2-j2KVIv2hYgkgHA/6pAadbXGlqKznf_n3_mrhVz_FV3TBZupiMZoFGVcc-Y\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/v5RRKQ2rrnaT7BpG6YxUmw/zWxV8QqQyxLtGH-wlTKTGdZAHM3HRqzuZ74rxapRfEvBlnUwJVho3_Xpk2_1oW9h0IB36D5KbRm4CUxM-eIRgQFMpY388-FgwQaOU5KkA3ooUc7z4AEgZNIH6DDKyincqfmw877fxGS5d7KpfwM3UiaQg8dUa9vt-aGFggnZWwodrcK_yO-_vnKqKvLd3Kk9/YJHZ_QmdwR5c_VB5qGD9lfOhjC3-ESOSVLaohvrGonU\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/6P0256TFuxT-dEjega0mcg/VI2e2WYIq4gWXnNIB_pBp4OXy3LF_OzCJF99RRdXflVOej6p_zLq2R6L5KWsMDOAFhqrbTcFJ-yt1pUdVKD7txrKWbb4TBCVXTPiaI3WG-fuysulumFqjgkhYSjv9UtAJNmqrln4KGPSRhwGhBVkh0TnAcUdWdsDiqRSGG29ZTnUakc2s3fWWx7nkJaUOpec/4owxPLxFK8qOiXKWCPljACDN_bNTycfKd2bnpjdirPE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1fzFnpP0RTA24_QM1glSbQ/4IrmXRvQ4jf8_MprceWnFX94gHzt_9XTWrhvUEkfeOHY3vqxe-fDnSUjW24PPeQROu7Vef2pyk4ctHGd4_mMc3EqnjvmTrupq1b_Z2jE8riLiDyqtCp1NntCWZJJP5B5kNOVIEytsqt-OJ1RHCoARF-kE-AKfHhOWUQxGr9hrQYDfS_E0HO7A69AdP3Y0N4F/qJ84hKf-ML33Z5W8iWO0M8Mysuid-00M3hQPDICpCzg\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/JfTwylMJVY-QmcZkPVsfSA/FP7ATfIkL5SOAHa1Cq6DylMn9V4q8wwMMlwRBYM1_OE2Agn7fB6PtxS0V0aaCKszQMmCO3nU5ysu6S-m80PoI8K1ojuMiGWC2QSLI2rX2IJuzlOJBuCX8kp04rbVYjbxVuyWk18Cx7cQnnhYFuaYa-n4CVSbKrKCNqYI5CvQAYaRSE8bgmGpoKlMzrBt_KZZ/Jk4gxMYhLz5qQPs_hSNszIygOY8ef27xyk_UzmrbsD8\",\"width\":600,\"height\":600}}}]}}");

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/" + MIYA_ANDO_RECORD_ID,
                HttpMethod.Get,
                fakeResponse);

            Task<AirtableRetrieveRecordResponse> task = airtableBase.RetrieveRecord(TABLE_NAME, MIYA_ANDO_RECORD_ID);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Record.Id == MIYA_ANDO_RECORD_ID);
            Assert.IsTrue(response.Record.GetField<string>("Name") == "Miya Ando");
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TjAtApiClientRetrieveRecordAttachmentsTest
        // Retrieve an existing record and knowning that this record has a field named 'Attachments'
        // for attachments. We subsequenly call record.GetAttachementList("Attachments") to help 
        // the end user to get a List<AirtableAttachment>.
        //
        // Any "empty" fields (e.g. "", [], or false) in the record will not be returned.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TjAtApiClientRetrieveRecordAttachmentsTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/GoJsfjcQ-dgcFx1F2d2qUw/tj4FG5snjzxkrAEI27jPzHReh5nKUm7Z1z2k9k0n8bm_ul_LRg9dlj2mfcKRk4f5/i5SZKJhHR86GKvzy-W-9tgr9dnN-jfllmnWyH461Z2A\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/7qrj9d1-WI6_IqprUuL63w/-kmzYAaVG8boiUupuKRXUV2kZQssGd-MVsnNb4nuUAktAyxTCayh5oFrkTp649TJ/D5F9EAY42r6dW5GTmmbZG9WPb6lll_yco8cPpiNAvuM\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HanQcO5l5itrnEoJvOpM4g/sq4hz2Z3vGUxNh35QRqMlRyNqZiyAQ_ghGjruuWpmCuU5QdzD32EUT263SqOsS4s/4eIhoSgxoOqQX1I0Wq3_gTG69ukcJeyEu0GBE_uQouQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/c93vRr5teXAGgw8p3WgPaQ/vxq-24t-JVpe_ZKg25DgAWn6k_tIkufS_6_mFsUGC2PbwyOEIIirwNMvCZTZXhDD/rzILsKoDIrMeIUahol5vFXAURgXil_uBERTaGkcR-qw\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zlOnaEgEDiB86LJaowingw/fA5SyRQtzRHtSBEMNJW7kSwq8GBtv1taJ1EFxRoiqqKBuri9V9aL8_0yu1Lpka0A7Apr1rE8K1BLT0ybBDBwRQ/tZQdKaGcqkRm2w1fvD2hWUbybMlEgHeQc9K7n1cu8tQ\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Kgs_SvSqybXg4M1tZJm5cw/QkVeqSQRN-sorTaB18pPaTTS_c0UXcTlWHz0zAzUR1Il4rXuRCREh-eqsiHq6UYSLfIP0VpgZPC-bbzKPf_ozw/DPgkFtrHUGhdcNzV2L9TRekA2qZLN-9kS_gAP0fTUgA\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/OpWL9ql-qRhYocT5kfV92w/XgElInhWNE8WcGUjxeipYp0IbTp9E44Kk8PdV_gQr2K9Wf9jN2p979qw-gsoCYgvylIhcU4E0_t53UyAuKVj0g/V74yqGMKqAPZN9w6zVwP9MaAPmbx3Al-il8U6c0sVrM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/V4ytQHf8bCihnMsJ7fMyiw/a3PAN-omfhxWqGxPRctUEthLXmaSkY2ZuLFtK7vD4pvXFupQ7xmWr3CKvyNEQZ_tAnckHKxCls9SEuLhMWwtNw/-s5YiZfOw0uKwCWZ97jJ-Y_8ieB36dhC-hil8ZHivhU\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zk3ktaxvd7aRG1of8BbOtw/MU0udt_s_iFNkVkw2KJwm2wnFdigLqQcWB5J29dAt4Ot1hBRizcO30tTfXDHkTf6sRpHQ2652SYKm9gbUNmGSZbGSCIKZEc-HF2cAQoJWiVqV-3xA6U3iaQkXJgfYniznYaM7J5WoeHL3JmcybhAtQ/3lJBdKA4SIcY0p2QyUgYw_F7aiCuoUIlHfWIC6v6Zps\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UwK69wQ8ZpIIkp7S4mpgEQ/EhAKnXfUtEigaMZ1--HYltrQRW_XIa2ZYe9XpUCNulVfLvv8yIRnf25YfnxoK814BpX38ZC65RKYtc7xDikmYA9LoIgOKwlo9z_9yqLwy2R22tKd6JilIcitNlI2H81xrrA2Uwz2tYhNOEimk80UsQ/4iy0EuV8951kQpwM67yAD2wRmHG53dErnD1lP5yUuwE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/T1eMM58E2Up60PALN-hIqw/_oYbjD7N7jImNwIfrPapfQ2YYcOAQSoGnL38hIsgdJ1t-qSDEzkFlrS6C3Lrx_2l4OXw_VuvK8TeX1R1Fcb6bbEy-JlAJIH0vTrr03Ct3EfptXriBPPpODPqtx_bMPHW8s4mo6CxXZRd4waEX7_qPw/HJdWG45nQDMGu9kK8pPXaOSYqEcB1YcQDgqqF1f4dRU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/u_3HMusgY3FmD0ntOTTAYw/-4vgpJgLKpaaH3WoiAW-XkiD20FXlt7eEzSBrnioT6F0rXIdXv_fGV_Edi8Z7ATW5ZNb70id-ASy8X2aZfenLuG6fLAwiyXoSib1kGF295eDQeYpRqb1eraQB4gPESadDorC-b2-j2KVIv2hYgkgHA/6pAadbXGlqKznf_n3_mrhVz_FV3TBZupiMZoFGVcc-Y\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/v5RRKQ2rrnaT7BpG6YxUmw/zWxV8QqQyxLtGH-wlTKTGdZAHM3HRqzuZ74rxapRfEvBlnUwJVho3_Xpk2_1oW9h0IB36D5KbRm4CUxM-eIRgQFMpY388-FgwQaOU5KkA3ooUc7z4AEgZNIH6DDKyincqfmw877fxGS5d7KpfwM3UiaQg8dUa9vt-aGFggnZWwodrcK_yO-_vnKqKvLd3Kk9/YJHZ_QmdwR5c_VB5qGD9lfOhjC3-ESOSVLaohvrGonU\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/6P0256TFuxT-dEjega0mcg/VI2e2WYIq4gWXnNIB_pBp4OXy3LF_OzCJF99RRdXflVOej6p_zLq2R6L5KWsMDOAFhqrbTcFJ-yt1pUdVKD7txrKWbb4TBCVXTPiaI3WG-fuysulumFqjgkhYSjv9UtAJNmqrln4KGPSRhwGhBVkh0TnAcUdWdsDiqRSGG29ZTnUakc2s3fWWx7nkJaUOpec/4owxPLxFK8qOiXKWCPljACDN_bNTycfKd2bnpjdirPE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1fzFnpP0RTA24_QM1glSbQ/4IrmXRvQ4jf8_MprceWnFX94gHzt_9XTWrhvUEkfeOHY3vqxe-fDnSUjW24PPeQROu7Vef2pyk4ctHGd4_mMc3EqnjvmTrupq1b_Z2jE8riLiDyqtCp1NntCWZJJP5B5kNOVIEytsqt-OJ1RHCoARF-kE-AKfHhOWUQxGr9hrQYDfS_E0HO7A69AdP3Y0N4F/qJ84hKf-ML33Z5W8iWO0M8Mysuid-00M3hQPDICpCzg\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/JfTwylMJVY-QmcZkPVsfSA/FP7ATfIkL5SOAHa1Cq6DylMn9V4q8wwMMlwRBYM1_OE2Agn7fB6PtxS0V0aaCKszQMmCO3nU5ysu6S-m80PoI8K1ojuMiGWC2QSLI2rX2IJuzlOJBuCX8kp04rbVYjbxVuyWk18Cx7cQnnhYFuaYa-n4CVSbKrKCNqYI5CvQAYaRSE8bgmGpoKlMzrBt_KZZ/Jk4gxMYhLz5qQPs_hSNszIygOY8ef27xyk_UzmrbsD8\",\"width\":600,\"height\":600}}}]}}");

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/" + MIYA_ANDO_RECORD_ID,
                    HttpMethod.Get,
                    fakeResponse);

            Task<AirtableRetrieveRecordResponse> task = airtableBase.RetrieveRecord(TABLE_NAME, MIYA_ANDO_RECORD_ID);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Record.Id == MIYA_ANDO_RECORD_ID);
            Assert.IsTrue(response.Record.GetField<string>("Name") == "Miya Ando");

            var attachmentList = response.Record.GetAttachmentField("Attachments");
            Assert.IsNotNull(attachmentList);
            Assert.IsTrue(attachmentList.Count() == 4);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TkAtApiClientRetrieveRecordTest_Template
        // Retrieve a record with a known record ID
        // <T> is <Artist> in this test.
        // The returnFieldsByFieldId flag (false by default) should not be enabled when using template.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TkAtApiClientRetrieveRecordTest_Template()
        {
            fakeResponse.Content = new StringContent
                ("{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/GoJsfjcQ-dgcFx1F2d2qUw/tj4FG5snjzxkrAEI27jPzHReh5nKUm7Z1z2k9k0n8bm_ul_LRg9dlj2mfcKRk4f5/i5SZKJhHR86GKvzy-W-9tgr9dnN-jfllmnWyH461Z2A\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/7qrj9d1-WI6_IqprUuL63w/-kmzYAaVG8boiUupuKRXUV2kZQssGd-MVsnNb4nuUAktAyxTCayh5oFrkTp649TJ/D5F9EAY42r6dW5GTmmbZG9WPb6lll_yco8cPpiNAvuM\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/HanQcO5l5itrnEoJvOpM4g/sq4hz2Z3vGUxNh35QRqMlRyNqZiyAQ_ghGjruuWpmCuU5QdzD32EUT263SqOsS4s/4eIhoSgxoOqQX1I0Wq3_gTG69ukcJeyEu0GBE_uQouQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/c93vRr5teXAGgw8p3WgPaQ/vxq-24t-JVpe_ZKg25DgAWn6k_tIkufS_6_mFsUGC2PbwyOEIIirwNMvCZTZXhDD/rzILsKoDIrMeIUahol5vFXAURgXil_uBERTaGkcR-qw\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zlOnaEgEDiB86LJaowingw/fA5SyRQtzRHtSBEMNJW7kSwq8GBtv1taJ1EFxRoiqqKBuri9V9aL8_0yu1Lpka0A7Apr1rE8K1BLT0ybBDBwRQ/tZQdKaGcqkRm2w1fvD2hWUbybMlEgHeQc9K7n1cu8tQ\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/Kgs_SvSqybXg4M1tZJm5cw/QkVeqSQRN-sorTaB18pPaTTS_c0UXcTlWHz0zAzUR1Il4rXuRCREh-eqsiHq6UYSLfIP0VpgZPC-bbzKPf_ozw/DPgkFtrHUGhdcNzV2L9TRekA2qZLN-9kS_gAP0fTUgA\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/OpWL9ql-qRhYocT5kfV92w/XgElInhWNE8WcGUjxeipYp0IbTp9E44Kk8PdV_gQr2K9Wf9jN2p979qw-gsoCYgvylIhcU4E0_t53UyAuKVj0g/V74yqGMKqAPZN9w6zVwP9MaAPmbx3Al-il8U6c0sVrM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/V4ytQHf8bCihnMsJ7fMyiw/a3PAN-omfhxWqGxPRctUEthLXmaSkY2ZuLFtK7vD4pvXFupQ7xmWr3CKvyNEQZ_tAnckHKxCls9SEuLhMWwtNw/-s5YiZfOw0uKwCWZ97jJ-Y_8ieB36dhC-hil8ZHivhU\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/zk3ktaxvd7aRG1of8BbOtw/MU0udt_s_iFNkVkw2KJwm2wnFdigLqQcWB5J29dAt4Ot1hBRizcO30tTfXDHkTf6sRpHQ2652SYKm9gbUNmGSZbGSCIKZEc-HF2cAQoJWiVqV-3xA6U3iaQkXJgfYniznYaM7J5WoeHL3JmcybhAtQ/3lJBdKA4SIcY0p2QyUgYw_F7aiCuoUIlHfWIC6v6Zps\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/UwK69wQ8ZpIIkp7S4mpgEQ/EhAKnXfUtEigaMZ1--HYltrQRW_XIa2ZYe9XpUCNulVfLvv8yIRnf25YfnxoK814BpX38ZC65RKYtc7xDikmYA9LoIgOKwlo9z_9yqLwy2R22tKd6JilIcitNlI2H81xrrA2Uwz2tYhNOEimk80UsQ/4iy0EuV8951kQpwM67yAD2wRmHG53dErnD1lP5yUuwE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/T1eMM58E2Up60PALN-hIqw/_oYbjD7N7jImNwIfrPapfQ2YYcOAQSoGnL38hIsgdJ1t-qSDEzkFlrS6C3Lrx_2l4OXw_VuvK8TeX1R1Fcb6bbEy-JlAJIH0vTrr03Ct3EfptXriBPPpODPqtx_bMPHW8s4mo6CxXZRd4waEX7_qPw/HJdWG45nQDMGu9kK8pPXaOSYqEcB1YcQDgqqF1f4dRU\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/u_3HMusgY3FmD0ntOTTAYw/-4vgpJgLKpaaH3WoiAW-XkiD20FXlt7eEzSBrnioT6F0rXIdXv_fGV_Edi8Z7ATW5ZNb70id-ASy8X2aZfenLuG6fLAwiyXoSib1kGF295eDQeYpRqb1eraQB4gPESadDorC-b2-j2KVIv2hYgkgHA/6pAadbXGlqKznf_n3_mrhVz_FV3TBZupiMZoFGVcc-Y\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/v5RRKQ2rrnaT7BpG6YxUmw/zWxV8QqQyxLtGH-wlTKTGdZAHM3HRqzuZ74rxapRfEvBlnUwJVho3_Xpk2_1oW9h0IB36D5KbRm4CUxM-eIRgQFMpY388-FgwQaOU5KkA3ooUc7z4AEgZNIH6DDKyincqfmw877fxGS5d7KpfwM3UiaQg8dUa9vt-aGFggnZWwodrcK_yO-_vnKqKvLd3Kk9/YJHZ_QmdwR5c_VB5qGD9lfOhjC3-ESOSVLaohvrGonU\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/6P0256TFuxT-dEjega0mcg/VI2e2WYIq4gWXnNIB_pBp4OXy3LF_OzCJF99RRdXflVOej6p_zLq2R6L5KWsMDOAFhqrbTcFJ-yt1pUdVKD7txrKWbb4TBCVXTPiaI3WG-fuysulumFqjgkhYSjv9UtAJNmqrln4KGPSRhwGhBVkh0TnAcUdWdsDiqRSGG29ZTnUakc2s3fWWx7nkJaUOpec/4owxPLxFK8qOiXKWCPljACDN_bNTycfKd2bnpjdirPE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/1fzFnpP0RTA24_QM1glSbQ/4IrmXRvQ4jf8_MprceWnFX94gHzt_9XTWrhvUEkfeOHY3vqxe-fDnSUjW24PPeQROu7Vef2pyk4ctHGd4_mMc3EqnjvmTrupq1b_Z2jE8riLiDyqtCp1NntCWZJJP5B5kNOVIEytsqt-OJ1RHCoARF-kE-AKfHhOWUQxGr9hrQYDfS_E0HO7A69AdP3Y0N4F/qJ84hKf-ML33Z5W8iWO0M8Mysuid-00M3hQPDICpCzg\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677988800000/JfTwylMJVY-QmcZkPVsfSA/FP7ATfIkL5SOAHa1Cq6DylMn9V4q8wwMMlwRBYM1_OE2Agn7fB6PtxS0V0aaCKszQMmCO3nU5ysu6S-m80PoI8K1ojuMiGWC2QSLI2rX2IJuzlOJBuCX8kp04rbVYjbxVuyWk18Cx7cQnnhYFuaYa-n4CVSbKrKCNqYI5CvQAYaRSE8bgmGpoKlMzrBt_KZZ/Jk4gxMYhLz5qQPs_hSNszIygOY8ef27xyk_UzmrbsD8\",\"width\":600,\"height\":600}}}]}}");

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/" + MIYA_ANDO_RECORD_ID,
                HttpMethod.Get,
                fakeResponse);

            Task<AirtableRetrieveRecordResponse<Artist>> task = airtableBase.RetrieveRecord<Artist>(TABLE_NAME, MIYA_ANDO_RECORD_ID);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Record.Id == MIYA_ANDO_RECORD_ID);

            // Abstract all fields of the record as an instance of Artist.
            Artist MiyaAndo = response.Record.Fields;

            Assert.AreEqual(MiyaAndo.Name, "Miya Ando");
            Assert.IsFalse(MiyaAndo.OnDisplay);
            Assert.AreEqual(MiyaAndo.Collection.Count, 1);
            Assert.AreEqual(MiyaAndo.Genre.Count, 2);
            Assert.IsNotNull(MiyaAndo.Bio);
            Assert.AreEqual(MiyaAndo.Attachments.Count, 4);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TlAtApiClientCreateRecordWithAttachmentsTest
        // Create a record
        // To create new attachments in Attachments, set the field value to an array of attachment objects.
        //
        // Any "empty" fields (e.g. "", [], or false) in the record will not be returned.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TlAtApiClientCreateRecordWithAttachmentsTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"id\":\"recasystgp3TLdTOs\",\"createdTime\":\"2023-03-05T02:14:47.000Z\",\"fields\":{\"Name\":\"Pablo Picasso\",\"Attachments\":[{\"id\":\"attho3YI7SpXeBwlh\",\"url\":\"https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\"}],\"Bio\":\"Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.\"}}");
            
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/",
                HttpMethod.Post,
                fakeResponse);

            // Create Attachments list
            var attachmentList = new List<AirtableAttachment>();
            attachmentList.Add(new AirtableAttachment { Url = "https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg"});

            var fields = new Fields();
            fields.AddField("Name", "Pablo Picasso");
            fields.AddField("Bio", "Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.");
    
            fields.AddField("Attachments", attachmentList);
            fields.AddField("On Display?", false);

            Task<AirtableCreateUpdateReplaceRecordResponse> task = airtableBase.CreateRecord(TABLE_NAME, fields, true);
            var response = await task;

            Assert.IsTrue(response.Success);

            Assert.IsTrue(response.Record.GetField<string>("Name") == "Pablo Picasso");

            Assert.IsTrue(response.Record.GetField<string>("Bio") == "Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.");

            Assert.IsNull(response.Record.GetField("On Display?"));
            var attListFromRecordCreated = response.Record.GetAttachmentField("Attachments");
            Assert.IsNotNull(attListFromRecordCreated);
            Assert.IsTrue(attListFromRecordCreated.Count() == 1);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TmAtApiClientUpdateRecordTest
        // Update a record
        // To update some(but not all) fields of a record.
        // Any fields that are not included will not be updated.
        //
        // This test makes sure that the attachments in the original record are retained
        // since this test does not update the Attchements field.
        //
        // To add attachments to Attachments, add new attachment objects to the existing array. 
        // Be sure to include all existing attachment objects that you wish to retain. 
        // For the new attachments being added, url is required, and filename is optional. 
        // To remove attachments, include the existing array of attachment objects, excluding any that you wish to remove.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TmAtApiClientUpdateRecordTest()
        {
            fakeResponse.Content = new StringContent
                //("{\"id\":\"recMx4H42jEjbZtu8\",\"fields\":{\"Name\":\"Pablo Picasso Updated\",\"Bio\":\"Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.\",\"Attachments\":[{\"id\":\"attULhWaWQLMnoB9J\",\"url\":\"https://dl.airtable.com/.attachments/828a69c57ed3c965db1193e8eb0d2568/67c0665c/Picasso_three_musicians_moma_2006.jpg\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\",\"size\":17604,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://dl.airtable.com/.attachmentThumbnails/329caf3b557ce24f6971c0761aa93056/02988635\",\"width\":41,\"height\":36},\"large\":{\"url\":\"https://dl.airtable.com/.attachmentThumbnails/312dd346acd7ca3a1fed1630c425eeaa/2b4d6f44\",\"width\":335,\"height\":296},\"full\":{\"url\":\"https://dl.airtable.com/.attachmentThumbnails/351c51cd2019de95ec2faace65c45c18/957d4007\",\"width\":3000,\"height\":3000}}}],\"On Display?\":true},\"createdTime\":\"2021-02-18T01:54:55.000Z\"}");
                ("{\"id\":\"recasystgp3TLdTOs\",\"createdTime\":\"2023-03-05T02:14:47.000Z\",\"fields\":{\"Bio\":\"Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.\",\"Name\":\"Pablo Picasso Updated\",\"Attachments\":[{\"id\":\"attho3YI7SpXeBwlh\",\"width\":335,\"height\":296,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677996000000/-uB3caDzoDOS7bC3VElr5Q/ZGJTMcDS4J4m-3hYVawnmRhJqxk4TOr9QQDYBx5D-O4BjOYPDxgg2hqQAQj-EEcsei1hwS8oJnfSC8BD4fSvZAUWBsL7o3XU3CE1ExvDgBqY1hAhGdObWhCvinxv18ZG/YHatyICqbkdbG1tDZv1eM1CDeY2vslpXpbqEWgYVtGI\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\",\"size\":17604,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677996000000/xbIay2L4bh3VmRvYkNr_eQ/NsVBKezNZ39nQhkRpN1_EPergCELUd-AVeIivmVDF0ejG0uHjblCcrQo1BTbsd3IjIYVxcbVz85LsD75kuLiKg/71Npr-vjhC5Mt6aaaIPVjeTSvfd8XRbDraEoXK9hHUQ\",\"width\":41,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677996000000/D51HamNfzIQs_tEmhv-cfA/Qw9ZoMfiVP8IPrzXIkWT4iiZM5SfB0EC1RBU5-hgEn6U_LskJxVweP3u36og59MxqPCjQhsGREyaPIU44HWHgQ/NScpSkSqX_L4Hdgz3U7FQoKpdIlCJ7ZKJqa3i3C4quc\",\"width\":335,\"height\":296},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1677996000000/wxOJA8uJKfY84Aa_-2cvkA/6jEg9QUfjCpxhI3fj6zJ0wyjEFXQOxRpJOzjp341T4iSzCYCFAHezP1FSNT04A46PcL_QrZVimtB-kADXRb5_g/wCsXWT31sOzQcdP-1MzMU7pNwwq5e2Ws0IN6GikfPQQ\",\"width\":3000,\"height\":3000}}}],\"On Display?\":true}}");
            
            string bodyText = "{\"fields\":{\"Name\":\"Pablo Picasso Updated\",\"On Display?\":true},\"typecast\":false}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/recasystgp3TLdTOs/",
                new HttpMethod("PATCH"),
                fakeResponse,
                bodyText);

            var fields = new Fields();
            fields.AddField("Name", "Pablo Picasso Updated");
            fields.AddField("On Display?", true);

            Task<AirtableCreateUpdateReplaceRecordResponse> task = airtableBase.UpdateRecord(TABLE_NAME, fields, "recasystgp3TLdTOs");
            var response = await task;

            Assert.IsTrue(response.Success);

            AirtableRecord record = response.Record;

            // the following 2 fields should be updated
            Assert.IsTrue(record.GetField<string>("Name") == "Pablo Picasso Updated");
            Assert.IsTrue(record.GetField<bool>("On Display?"));

            var attachmentListFromUpdatedRecord = record.GetAttachmentField("Attachments");
            Assert.IsNotNull(attachmentListFromUpdatedRecord);
            Assert.AreEqual(attachmentListFromUpdatedRecord.Count(), 1);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TnAtApiClientUpdateRecordRemoveAttachmentsTest
        // Update a record
        // To update some(but not all) fields of a record.
        // Any fields that are not included will not be updated.
        //
        // This test makes sure all the attachements in the record removed after
        // the update operation.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TnAtApiClientUpdateRecordRemoveAttachmentsTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"id\":\"recasystgp3TLdTOs\",\"createdTime\":\"2023-03-05T02:14:47.000Z\",\"fields\":{\"On Display?\":true,\"Bio\":\"Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.\",\"Name\":\"Pablo Picasso Updated\"}}");

            string bodyText = "{\"fields\":{\"Name\":\"Pablo Picasso Updated\",\"On Display?\":true,\"Attachments\":[]},\"typecast\":false}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/recasystgp3TLdTOs/",
                new HttpMethod("PATCH"),
                fakeResponse,
                bodyText);

            var attachmentList = new List<AirtableAttachment>();
            var fields = new Fields();
            fields.AddField("Name", "Pablo Picasso Updated");
            fields.AddField("On Display?", true);
            fields.AddField("Attachments", attachmentList);

            Task<AirtableCreateUpdateReplaceRecordResponse> task = airtableBase.UpdateRecord(TABLE_NAME, fields, "recasystgp3TLdTOs");
            var response = await task;

            Assert.IsTrue(response.Success);

            Assert.IsTrue(response.Record.GetField<string>("Name") == "Pablo Picasso Updated");
            Assert.IsTrue(response.Record.GetField<bool>("On Display?"));

            var attachmentListFromUpdatedRecord = response.Record.GetAttachmentField("Attachments");
            Assert.IsNull(attachmentListFromUpdatedRecord);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.ToAtApiClientReplaceRecordTest
        // Replace a record
        // Any fields that are not included will be cleared.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task ToAtApiClientReplaceRecordTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"id\":\"recasystgp3TLdTOs\",\"createdTime\":\"2023-03-05T02:14:47.000Z\",\"fields\":{\"On Display?\":true,\"Name\":\"Pablo Picasso Replaced\",\"Attachments\":[{\"id\":\"attEVx2vJRMeI81kN\",\"url\":\"https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\"},{\"id\":\"att4eQmYpP9nvBtdN\",\"url\":\"https://upload.wikimedia.org/wikipedia/en/thumb/6/6a/Pablo_Picasso%2C_1921%2C_Nous_autres_musiciens_%28Three_Musicians%29%2C_oil_on_canvas%2C_204.5_x_188.3_cm%2C_Philadelphia_Museum_of_Art.jpg/800px-Pablo_Picasso%2C_1921%2C_Nous_autres_musiciens_%28Three_Musicians%29%2C_oil_on_canvas%2C_204.5_x_188.3_cm%2C_Philadelphia_Museum_of_Art.jpg\",\"filename\":\"800px-Pablo_Picasso%2C_1921%2C_Nous_autres_musiciens_%28Three_Musicians%29%2C_oil_on_canvas%2C_204.5_x_188.3_cm%2C_Philadelphia_Museum_of_Art.jpg\"}]}}");
            
            string bodyText = "{\"fields\":{\"Name\":\"Pablo Picasso Replaced\",\"On Display?\":true,\"Attachments\":[{\"url\":\"https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg\"},{\"url\":\"https://upload.wikimedia.org/wikipedia/en/thumb/6/6a/Pablo_Picasso%2C_1921%2C_Nous_autres_musiciens_%28Three_Musicians%29%2C_oil_on_canvas%2C_204.5_x_188.3_cm%2C_Philadelphia_Museum_of_Art.jpg/800px-Pablo_Picasso%2C_1921%2C_Nous_autres_musiciens_%28Three_Musicians%29%2C_oil_on_canvas%2C_204.5_x_188.3_cm%2C_Philadelphia_Museum_of_Art.jpg\"}]},\"typecast\":false}";
            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/recasystgp3TLdTOs/",
                    HttpMethod.Put,
                    fakeResponse,
                    bodyText);

            var fields = new Fields();
            fields.AddField("Name", "Pablo Picasso Replaced");
            fields.AddField("On Display?", true);
            var attachment1 = new AirtableAttachment { Url = "https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg"};
            var attachment2 = new AirtableAttachment { Url = "https://upload.wikimedia.org/wikipedia/en/thumb/6/6a/Pablo_Picasso%2C_1921%2C_Nous_autres_musiciens_%28Three_Musicians%29%2C_oil_on_canvas%2C_204.5_x_188.3_cm%2C_Philadelphia_Museum_of_Art.jpg/800px-Pablo_Picasso%2C_1921%2C_Nous_autres_musiciens_%28Three_Musicians%29%2C_oil_on_canvas%2C_204.5_x_188.3_cm%2C_Philadelphia_Museum_of_Art.jpg"};
            var attachmentList = new List<AirtableAttachment>();
            attachmentList.Add(attachment1);
            attachmentList.Add(attachment2);
            fields.AddField("Attachments", attachmentList);

            Task<AirtableCreateUpdateReplaceRecordResponse> testRecordTask = airtableBase.ReplaceRecord(TABLE_NAME, fields, "recasystgp3TLdTOs");
            var response = await testRecordTask;
            Assert.IsTrue(response.Success);

            Assert.IsTrue(response.Record.GetField<string>("Name") == "Pablo Picasso Replaced");
            Assert.IsTrue(response.Record.GetField<bool>("On Display?"));

            var attachmentListFromReplacedRecord = response.Record.GetAttachmentField("Attachments");
            Assert.IsNotNull(attachmentListFromReplacedRecord);
            Assert.IsTrue(attachmentListFromReplacedRecord.Count() == 2);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TpAtApiClientDeleteRecordTest
        // Delete a record
        //"{\"deleted\":true,\"id\":\"recMx4H42jEjbZtu8\"}"
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TpAtApiClientDeleteRecordTest()
        {
            fakeResponse.Content = new StringContent("{\"deleted\":true,\"id\":\"recasystgp3TLdTOs\"}");

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/recasystgp3TLdTOs",
                HttpMethod.Delete,
                fakeResponse);

            Task<AirtableDeleteRecordResponse> task = airtableBase.DeleteRecord(TABLE_NAME, "recasystgp3TLdTOs");
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Deleted);
            Assert.IsTrue(response.Id == "recasystgp3TLdTOs");
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TqAtApiClientCreateOneRecordWithAttachmentsInArrayTest
        // Create a record using the batch create API.
        //
        // Any "empty" fields (e.g. "", [], or false) in the record will not be returned.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TqAtApiClientCreateOneRecordWithAttachmentsInArrayTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recFoLxTZ5UJ1c7qf\",\"createdTime\":\"2023-03-05T20:51:14.000Z\",\"fields\":{\"Name\":\"Pablo Picasso\",\"Attachments\":[{\"id\":\"attqMlBGnfqCzGZn0\",\"url\":\"https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\"}],\"Bio\":\"Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.\"}}]}");
            
            string bodyText = ("{\"records\":[{\"fields\":{\"Name\":\"Pablo Picasso\",\"Bio\":\"Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.\",\"Attachments\":[{\"url\":\"https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg\"}],\"On Display?\":false}}],\"typecast\":true}");
            
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/",
                HttpMethod.Post,
                fakeResponse, 
                bodyText);

            // Create Attachments list
            var attachmentList = new List<AirtableAttachment>();
            attachmentList.Add(new AirtableAttachment { Url = "https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg"});

            Fields[] fields = new Fields[1];
            fields[0] = new Fields();
            fields[0].AddField("Name", "Pablo Picasso");
            fields[0].AddField("Bio", "Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.");

            fields[0].AddField("Attachments", attachmentList);
            fields[0].AddField("On Display?", false);

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.CreateMultipleRecords(TABLE_NAME, fields, true);
            var response = await task;

            Assert.IsTrue(response.Success);

            Assert.IsTrue(response.Records[0].GetField<string>("Name") == "Pablo Picasso");
            Assert.IsTrue(response.Records[0].GetField<string>("Bio") == "Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.");

            Assert.IsFalse(response.Records[0].GetField<bool>("On Display?"));  // because "On Display?" is null, default to false

            var attListFromRecordCreated = response.Records[0].GetAttachmentField("Attachments");
            Assert.IsNotNull(attListFromRecordCreated);
            Assert.IsTrue(attListFromRecordCreated.Count() == 1);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TrAtApiClientUpdateOneRecordInArrayTest
        // Update one record using the batch Update API
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TrAtApiClientUpdateOneRecordInArrayTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recFoLxTZ5UJ1c7qf\",\"createdTime\":\"2023-03-05T20:51:14.000Z\",\"fields\":{\"Bio\":\"Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.\",\"Name\":\"Pablo Picasso\",\"Attachments\":[{\"id\":\"attqMlBGnfqCzGZn0\",\"width\":335,\"height\":296,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678068000000/cbNC-qdl-I_AOP5N6q3z7Q/Lj-WV2l4DYN4YIydMP_df2fsdp8gSp8pU74QnxUSF9YzVu93bVgPw5jF4W-gS3xCLR05RdhiOiy8aQ26_ZJ1q66ueVKb61CIPfGIuYKnG7dTyhqqdalovmkRbv1jV7OP/QJM5gMMDxa1BLWp4ftvg4aoA6lhpatrBptUlyIug14g\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\",\"size\":17604,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678068000000/0N8k6W5IFhlfn8FFjFQyrw/HogrgBnU6gGINwYUjI3k-b_8idKFKtK-l-TqhUUKYnVSE-zevOtNUqDmsngK4qtbnReuebEMRdAPnXfPCj8iVA/_hXq58Co03undMQaJfGGHcpUEc6Umu7Vg2ozV6zEAFI\",\"width\":41,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678068000000/I_uPhf5ID9c2S8T3zswkvw/Urw9CAM_KBoScvdskjkPpXhBCSeM4IrXCk1eH7b9sUPM2D6wi8xQ87sJc9FiC3mvNjiszAa5RRNa2GJwR695cg/RsOU10hBrI3iLEAemkNyjg_JD5JNdFTbfdu1veJZQIw\",\"width\":335,\"height\":296},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678068000000/lurpN-_M9i3_2JJ0xL7VRQ/q5_tUn26EkFjy9rY7NmkmkNSsScGAsjIZd9n03LbAe4IYUl1Kv5ZzYo276_7lzuMLCWssS8aqoytjiB-eGWgKw/z8rF1Uu_dynJhSTALzTya2lk9TFj4ZP7RuTYXvG-DgY\",\"width\":3000,\"height\":3000}}}],\"On Display?\":true}}]}");
            
            string bodyText = "{\"returnFieldsByFieldId\":false,\"typecast\":false,\"records\":[{\"id\":\"recFoLxTZ5UJ1c7qf\",\"fields\":{\"On Display?\":true}}]}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/",
                    new HttpMethod("PATCH"),
                    fakeResponse,
                    bodyText);

            string errorMessage = null;
            IdFields[] idFields = new IdFields[1];
            idFields[0] = new IdFields("recFoLxTZ5UJ1c7qf");
            idFields[0].AddField("On Display?", true);
            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.UpdateMultipleRecords(TABLE_NAME, idFields);
            var response = await task;
            AirtableRecord[] records = null;
            if (!response.Success)
            {
                if (response.AirtableApiError is AirtableApiException)
                {
                    errorMessage = response.AirtableApiError.ErrorMessage;
            }
                else
                {
                    errorMessage = "Unknown error";
                }
            }
            else
            {
                records = response.Records;

                // the following field should be updated
                Assert.IsTrue(response.Records[0].GetField<bool>("On Display?"));

                // the following 2 fields should be unchanged
                Assert.IsTrue(records[0].GetField<string>("Name") == "Pablo Picasso");
                Assert.IsTrue(records[0].GetField<string>("Bio") == "Spanish expatriate Pablo Picasso was one of the greatest and most influential artists of the 20th century, as well as the co-creator of Cubism.");
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine(errorMessage);
            }
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }



        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TsAtApiClientReplaceOneRecordTest
        // Replace one record using the batch Replace API
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TsAtApiClientReplaceOneRecordTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recFoLxTZ5UJ1c7qf\",\"createdTime\":\"2023-03-05T20:51:14.000Z\",\"fields\":{\"Name\":\"Auguste Rodin\"}}]}");

            string bodyText = "{\"returnFieldsByFieldId\":false,\"typecast\":false,\"records\":[{\"id\":\"recFoLxTZ5UJ1c7qf\",\"fields\":{\"Name\":\"Auguste Rodin\"}}]}";
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/",
                HttpMethod.Put,
                fakeResponse,
                bodyText);

            string errorMessage = null;
            IdFields[] idFields = new IdFields[1];
            idFields[0] = new IdFields("recFoLxTZ5UJ1c7qf");
            idFields[0].AddField("Name", "Auguste Rodin");
            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.ReplaceMultipleRecords(TABLE_NAME, idFields);
            var response = await task;
            AirtableRecord[] records = null;
            if (!response.Success)
            {
                if (response.AirtableApiError is AirtableApiException)
                {
                    errorMessage = response.AirtableApiError.ErrorMessage;
                }
                else
                {
                    errorMessage = "Unknown error";
                }
            }
            else
            {
                records = response.Records;

                // the following field should be updated
                Assert.IsFalse(records[0].GetField<bool>("On Display?"));  // false for null value 

                Assert.IsNull(records[0].GetField("Bio?"));

                // the following field should be unchanged
                Assert.IsTrue(records[0].GetField<string>("Name") == "Auguste Rodin");
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine(errorMessage);
            }
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TtAtApiClientCreateMultipleRecordsTest
        // Create multiple records in one single operation using the batch create API.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TtAtApiClientCreateMultipleRecordsTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recYQ8VMRehJ0MdpD\",\"createdTime\":\"2023-03-05T23:31:23.000Z\",\"fields\":{\"Name\":\"Claude Monet\",\"Bio\":\"Oscar - Claude Monet was a French painter, a founder of French Impressionist painting and the most consistent and prolific practitioner of the movement philosophy of expressing perceptions before nature, especially as applied to plein air landscape painting\"}},{\"id\":\"rec02o2lCQsq0VFvU\",\"createdTime\":\"2023-03-05T23:31:23.000Z\",\"fields\":{\"Name\":\"Vincent van Gogh\",\"Bio\":\"Vincent Willem van Gogh was a Dutch post-impressionist painter who is among the most famous and influential figures in the history of Western art. In just over a decade he created about 2,100 artworks, including around 860 oil paintings, most of them in the last two years of his life.\"}}]}");
            
            string bodyText = "{\"records\":[{\"fields\":{\"Name\":\"Claude Monet\",\"Bio\":\"Oscar - Claude Monet was a French painter, a founder of French Impressionist painting and the most consistent and prolific practitioner of the movement philosophy of expressing perceptions before nature, especially as applied to plein air landscape painting\"}},{\"fields\":{\"Name\":\"Vincent van Gogh\",\"Bio\":\"Vincent Willem van Gogh was a Dutch post-impressionist painter who is among the most famous and influential figures in the history of Western art. In just over a decade he created about 2,100 artworks, including around 860 oil paintings, most of them in the last two years of his life.\"}}],\"typecast\":true}";
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            Fields[] fields = new Fields[2];
            fields[0] = new Fields();
            fields[0].AddField("Name", "Claude Monet");
            fields[0].AddField("Bio", "Oscar - Claude Monet was a French painter, a founder of French Impressionist painting and the most consistent and prolific practitioner of the movement philosophy of expressing perceptions before nature, especially as applied to plein air landscape painting");

            fields[1] = new Fields();
            fields[1].AddField("Name", "Vincent van Gogh");
            fields[1].AddField("Bio", "Vincent Willem van Gogh was a Dutch post-impressionist painter who is among the most famous and influential figures in the history of Western art. In just over a decade he created about 2,100 artworks, including around 860 oil paintings, most of them in the last two years of his life.");

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.CreateMultipleRecords(TABLE_NAME, fields, true);
            var response = await task;

            Assert.IsTrue(response.Success);

            Assert.IsTrue(response.Records[0].GetField<string>("Name") == "Claude Monet");

            Assert.IsTrue(response.Records[0].GetField<string>("Bio") == "Oscar - Claude Monet was a French painter, a founder of French Impressionist painting and the most consistent and prolific practitioner of the movement philosophy of expressing perceptions before nature, especially as applied to plein air landscape painting");

            Assert.IsTrue(response.Records[1].GetField<string>("Name") == "Vincent van Gogh");

            Assert.IsTrue(response.Records[1].GetField<string>("Bio") == "Vincent Willem van Gogh was a Dutch post-impressionist painter who is among the most famous and influential figures in the history of Western art. In just over a decade he created about 2,100 artworks, including around 860 oil paintings, most of them in the last two years of his life.");
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TuAtApiClientUpdateMultipleRecordstest
        // Update multiple records in one single operation using the batch Update API.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TuAtApiClientUpdateMultipleRecordstest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec9SVBYo2VdmRFeL\",\"createdTime\":\"2023-03-06T00:40:14.000Z\",\"fields\":{\"Name\":\"Claude Monet\",\"Bio\":\"Oscar - Claude Monet was a French painter, a founder of French Impressionist painting and the most consistent and prolific practitioner of the movement philosophy of expressing perceptions before nature, especially as applied to plein air landscape painting\",\"On Display?\":true}},{\"id\":\"recGIyNcNw3PHFF1J\",\"createdTime\":\"2023-03-06T00:40:14.000Z\",\"fields\":{\"Name\":\"UpdatedNameVincentVanGogh\",\"Bio\":\"Vincent Willem van Gogh was a Dutch post-impressionist painter who is among the most famous and influential figures in the history of Western art. In just over a decade he created about 2,100 artworks, including around 860 oil paintings, most of them in the last two years of his life.\"}}]}");
            
                string bodyText = "{\"returnFieldsByFieldId\":false,\"typecast\":false,\"records\":[{\"id\":\"rec9SVBYo2VdmRFeL\",\"fields\":{\"On Display?\":true}},{\"id\":\"recGIyNcNw3PHFF1J\",\"fields\":{\"Name\":\"UpdatedNameVincentVanGogh\"}}]}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/",
                new HttpMethod("PATCH"),
                fakeResponse,
                bodyText);

            IdFields[] idFields = new IdFields[2];
            idFields[0] = new IdFields("rec9SVBYo2VdmRFeL");
            idFields[0].AddField("On Display?", true);

            idFields[1] = new IdFields("recGIyNcNw3PHFF1J");
            idFields[1].AddField("Name", "UpdatedNameVincentVanGogh");

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.UpdateMultipleRecords(TABLE_NAME, idFields);
            var response = await task;

            Assert.IsTrue(response.Success);
            AirtableRecord[] records = response.Records;

            // the following 2 fields should be updated
            Assert.IsTrue(records[0].GetField<bool>("On Display?"));
            Assert.IsTrue(response.Records[1].GetField<string>("Name") == "UpdatedNameVincentVanGogh");

            // Id should be unchanged
            Assert.IsTrue(records[0].Id == "rec9SVBYo2VdmRFeL");
            Assert.IsTrue(records[1].Id == "recGIyNcNw3PHFF1J");
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TvAtApiClientReplaceMultipleRecordsTest
        // Replace multiple records in one single operation using the batch Replace API
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TvAtApiClientReplaceMultipleRecordsTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recjwUAWZ9dHKWsUl\",\"createdTime\":\"2023-03-06T01:24:10.000Z\",\"fields\":{\"Name\":\"Claude Monet\",\"On Display?\":true}},{\"id\":\"recwVd14KhdP5BO56\",\"createdTime\":\"2023-03-06T01:24:10.000Z\",\"fields\":{\"Name\":\"Vincent VanGogh replaced\"}}]}");            
            
            string idMonet = "recjwUAWZ9dHKWsUl";
            string idVanGogh = "recwVd14KhdP5BO56";

            string bodyText = "{\"returnFieldsByFieldId\":false,\"typecast\":false,\"records\":[{\"id\":\"recjwUAWZ9dHKWsUl\",\"fields\":{\"Name\":\"Claude Monet\",\"On Display?\":true}},{\"id\":\"recwVd14KhdP5BO56\",\"fields\":{\"Name\":\"Vincent VanGogh replaced\"}}]}";
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/",
                HttpMethod.Put,
                fakeResponse,
                bodyText);

            string errorMessage = null;

            IdFields[] idFields = new IdFields[2];
            idFields[0] = new IdFields(idMonet);
            idFields[0].AddField("Name", "Claude Monet");
            idFields[0].AddField("On Display?", true);

            idFields[1] = new IdFields(idVanGogh);
            idFields[1].AddField("Name", "Vincent VanGogh replaced");

            AirtableRecord[] records = null;
            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.ReplaceMultipleRecords(TABLE_NAME, idFields);
            var response = await task;

            if (!response.Success)
            {
                if (response.AirtableApiError is AirtableApiException)
                {
                    errorMessage = response.AirtableApiError.ErrorMessage;
                }
                else
                {
                    errorMessage = "Unknown error";
                }
            }
            else
            {
                errorMessage = null;
                records = response.Records;

                // the following 2 fields should be updated
                Assert.IsTrue(records[0].GetField<bool>("On Display?"));
                Assert.IsTrue(records[1].GetField<string>("Name") == "Vincent VanGogh replaced");

                // the following 4 fields should be unchanged
                Assert.IsTrue(records[0].Id == idMonet);
                Assert.IsTrue(records[1].Id == idVanGogh);
                Assert.IsTrue(records[0].GetField<string>("Name") == "Claude Monet");
                Assert.IsFalse(records[1].GetField<bool>("On Display?"));  // onDisplay does not exist
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine(errorMessage);
            }
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TwAtApiClientListRecordsReturnFieldsFieldIdTest
        // List records
        // Returned records do not include any fields with "empty" values, e.g. "", [], or false.
        // Field Id instead of Field Name will be returned for the returned fields of each list in the record list.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TwAtApiClientListRecordsReturnFieldsFieldIdTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"fld2JydboQu8WI3YD\":true,\"fldE0muAk6ejOkkKa\":[\"American Abstract Expressionism\",\"Color Field\"],\"fldLSIVsf7fMtOgIv\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"fldSAUw6qVy9NzXzF\":\"Al Held\",\"fldTNbsJI0zoZbdUP\":\"1970-11-29T03:00:00.000Z\",\"fldmUsXju7aIztvI2\":[\"recuV4lqy2awmYEVq\"],\"flduyfQKyx8QQ8r4j\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/37trmUVlDHScolF_1ZwqPg/bfnuoP5c5-35acYNFXLWwPicuB0GQpED3c5G61hWrhuw3bq3aW2DJZNbxYy1Z2GW/3GfwltZG1ySA56ong6nK3x5i-1BtY5ty4A41wVRZGpg\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/am0KuuU_wo0d-uZy-K9bNA/7052dgh9CBze8Ff0YQ4vbRCl79vvU_RS7E2UphaKbOgKCjx0wvXd-lFwCjr7YWwl1QScagC-r3VdPDMf013IGQ/25iHLWAKJ5jE-4EsLreXYNdLHRydlCdQLkqbIxa-sgY\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/KaNdRxq9E9JlBQ52t2wPSA/bwMMpd4juZXEoMwxC7XllDUSLujW5WnNo_EsDVEGEhE8xrZ0E8KxpepUv-w8Nsa0E-Y3NOd3oo80v07uaUFiXA/XiBqp_pGPydyoFykV_1ZsbbwxkGS-5Il6hnxtLKll0A\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/XaBszxHSrpJ4xIH7uR9nlQ/EoGMvLFx2zXy8FxPYyicb7nVdvzV6MiX27LBDjQAo3YKODQ4YvX2FTQPi7FTaCT23jXlN7l_JvnQJ-AuUyQPlw/u1_6QfaxZ1E5tm-GAexi5hzdGoDnk-bnKgTAymtOEIU\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/EEh_6p6cro80V4JbDFvfYQ/YCCQFshPV3atpUw3beV1zfZulo1yUazNV9OitSJJFyJlSLwbIeblQptmxDd_Jawm/KmFR4wLH3IcCYMVsRYwjqcSBQGocBJvJpXB5VCjruws\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/hath8K79MZkGxhKAPmOn0g/13md7tTu5oZdBabckxRpPFr6Rq17XgU8qIRGVGH2SPLOqVwDl_sL4xxxehrJS_BB/KWOWF4OD9vWkFk2-Vz8LGvDNrW2Q1Ds7tBf2R69YAu4\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/a9--oUkoFbHq7InYrmiFgQ/upzJZGs0FVMP_KOzxlEUy3FazGMX3XFtkaNXay31fPGBsZg7KOETSk_IdHxNYmNQ/_pKVMjXMnCbgUSnqGpEukddOfavqAnwzU7ZF8yFvjI4\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/U2_dmgHW6M9L_BPO34BRlg/tmsU5Fh_3V25KmCqXBZ2EkR924-HACXuJNnHQT0gPb19XQhqCb-c7sb24wnJ0cuB/MxckBQNlTKVusDpCm_Xq0IBz8ru6T965p03d_gVXAho\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/D6c4lPxlnrQSnCj_DfJtJw/6gzYep14_EFQNPKF_CM4U7ZawfS2Tm5d1R1dyxGdxZTe7aBfCblkKrpxhfcrB2Rc/8HqQ0-yVx2sL7GkBfpaaYLLlUWmzcIKsJrxJ7IdPCOw\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/RmNZEbYn9NbAlyQivBiT-Q/7TeoPx2xT8s0K1lxVlQDeeGcXRI1Bk9_F8Lu1G0h9EkLZ08w8fum2dunSTFE5-uz/UlOgVXTbP25INR5_caatziDMF3FKi3uwBs23Q9bM4ag\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/FwzhggrCwqLB6EiZvTJVBg/nSVSo3XlGA8L-17IFv1XeRvq3UR2ZFFJUGZGpWwhelCdk9SE-QVJTbYF3HWfvtp5/2qLLwCbZ4vqparZMS9zvBy17K9HG_OGO2ylKsyhp12Y\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/TmipyAWfbWHvneR7J6-KDA/d92LMvKRiuK6vRWLAjZnpViyO6guBiTo3WjuM0HMoJO4AsnoXHrQNIYcdKdtdgnr/mD1BCJPCQAop6dT7UQFNlw5p9Q3wWYxelw2i1Zs4DQs\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/X2oeZ-YnzM50TGM5hgUCDw/ixQUiQc9atkTw7RjphkbziDzSMmM7OWhwNUlxCzPqHw4H_RRt_Yuo8BtLfEfpKmc/t5-E0Md0puCz6iPCA9_GpeDpvsp3Akg_FJ44xvZKcM0\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/DPLHhNnWBikHS60m6r-cQw/p6zGXqOCX4S_OMQ6hXDDrC3v4O_kxYnPDRnaKotgn7Fhk0EQU2ftYuzUftcDYvpB/6ClJEZaHHpB4xIw55_Ik7ZkEyZfGrQ1msmpDkCBRCY0\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/eEinMd3nUshvK83_w5RUrg/pwcgnl2kTuP5kNrHquaAXB50Ca_LEKdpTFZytAC0dMm7JrcEp3RNr25AyDJMZgn4/6dTvm-7v8z_3IgFnMUFXOSKtOiLNbS54-VkgjpQoxIY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/tx-mdGwn-hERK5peCZ5JJg/ytzMcRurIDRPPkw8FUvHYN1TR4Cc2VN45N7UQJBw8oz5-NrVP_jfrjXcaFLaUa6j/xsX8lfz0fHyrHS7rA9Zr481JvgcEVwrJ0y8OskOVRzU\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"fld2JydboQu8WI3YD\":true,\"fldE0muAk6ejOkkKa\":[\"Abstract Expressionism\",\"Modern art\"],\"fldLSIVsf7fMtOgIv\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\\\\\\\\\n\\\\\\\\\\n\",\"fldSAUw6qVy9NzXzF\":\"Arshile Gorky\",\"fldmUsXju7aIztvI2\":[\"recuV4lqy2awmYEVq\"],\"flduyfQKyx8QQ8r4j\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/u1KNZlClL-Of3ZDkZJiQ4A/rNP3W_5VqRfIlvmBg9OMh15jpJpxHnFIfNGin9HZyAfqSBlQCXTPnYiriP5TLaKw/DWLHzPun1y1bPfpBCO70HBRsW_0VQzmY0-vSVkcX3Xs\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/GESS6dWpPwaRyKaJ_o7LAA/rtKHQfJ8obIx8zqxXNK-RIfdE4DUhG7rBC9s35pu296SQuF1tL1gy90Y7eBg5fJY/lBkC4Sf-iozDN4Xw7-0MlpM4Us9dUcBsZI0sxaydpDo\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/bKJy4pAk5iTb6NkCpOHKrw/m4_uTA50lhRtMlA1TYp9UsCERyPif9psTC_UsphR4ircFIbSmHsJpx_F0zOpGSHM/S_dF_IaRdqXz7J9IDzFZQGJz8PFBZ0UIbW0ZVpKbEZM\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/YyYpokDvQ72pivqt6NYCRA/EYxPaMksYGiiRUtZINMFdJevEdJcOzMSQ6O0mood5PcWn9kdjb0Z_ZEcaGRo3xNg/cf7-rbR7u16G73WJ4vYe9ae5t9xAayRkAXD3mktJEhQ\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/XpHlEuuT9HVXaSd9AtesaA/7l9KEZpiTvCGfcz8od-tvjigcPz4TRQtQHDvWSYXOSt_8MujM05n5E6wzYmY5gUOMPrLvFhQ99vyWIBIjRdQOA/1opWe-7QQivN39pVw3FPMLApgEWLzO-9tanLzpffzHs\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/Q3pWJBnIVr9rUfgSvkW1rA/3lg6ZqhjbV_XdztfDmq_m9MNKDBWPZjaZxWrqR4pGL_eonP7rxrUEdlokp6joMh4JBAKBImGajhnWB5x0x0CDw/JktpG2QSYAs4qvnXnYL_vR8m5sD249SL7wcmMlb9Rf0\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/3-Sie76ffUAz7CMiVemVag/Io0deK605OZV0qAvGLa_MJEQKNc4cpw1hfWrhzdyNKFWnzg9SEJbq7dvUM_azUo8NksKlFAbLc6kSvBMIIeNGg/ZA_wBOqwDS2Ih2L4xsJgqORnzd6jCcL_NYQ77ugU5A0\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/ZkaDOkhOsoYL-CNANt8-4Q/KfBbyIsgbPCcrieWFwL4Cz3WhfC2TOKrCEfU75LtGDzimxIuwldhJmMryYRibcPNHEJPaB5CSj7Jmgb5jeeiFQ/Vz1Doh95xw51IC0k3WoQnUO7YooYlA_94U8IrMBXorw\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/n8j8vGsgDv124CcaZXbREQ/5Mja8IHD4PcXvSIXzAhIcXUHa2Ndh0gRcqb28BUTL1poKaqCSer0offpe6W_rgBY/O5z3ySOMTRW4zUW4RXG2Z5496GcV1kMGb_je02BS258\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/AjaY8P_5uavLsTbO-9P9EA/FxZd5CeV08PajrJH2Gl63ZQEz4zqwVOa3rpUyWa86kNuOMrbAy4bscCFyWd91mkLrUiCNlKhwGHi0ZSQgDNzCg/Ng20_7ywL7aRzJanFnr4cMokpjoVveGaVcwtNxZv7Fg\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/4vGuZPnE6p6I_SLh_a_vdA/evjBy-BIWCsdUXjIkhYpYW2f4dNnI0Zo3QhpZfPgZqO0Sicv88sZ-G_-IZ1pptMInRfnLo5cJ6lpz3EfDKrXgQ/uD1bD8xgCTWYTeIh76C6hEXMqgDcc7uMvvWWcdVl9Oo\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/oSEQvOFEo2z3FG6J3YHoQw/pgf6eyd4CfHaTzR-uwmsyQpcLNJWdanIwWwx-l1mQB21n7KyK5fGLAvOYdM3xp-dJFWkWONziS0U0-tY1VGDiA/A70ZQCC_qpELQsg-oLkpxyQkSouyEaZnjFZLvSbMLDo\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"fldE0muAk6ejOkkKa\":[\"Post-minimalism\",\"Color Field\"],\"fldLSIVsf7fMtOgIv\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"fldSAUw6qVy9NzXzF\":\"Miya Ando\",\"fldmUsXju7aIztvI2\":[\"recoOI0BXBdmR4JfZ\"],\"flduyfQKyx8QQ8r4j\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/ROOBR6c8d8IAavi7hCZdpg/qH73VRQ9wyFq0FCRNAIssAkdLi7cP1mCIDuVC2j8FTwcACSoAsgfcHcO1feHNQW3/Lim8cKaTyl2yTIJiKPhlI_4c6gRfdyAUWCG48-VVajo\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/D5EEXkLy9qUzmVak3FuzfA/0w8W9gbKm7s3pkmxfpZeL-HDeBGhF1fnNTGjl2lCosHmKnkhkri7G-a6wQG7uPAC/bM-tlbNqS0gGVvLl3xVFXM1PRrKrtL60ZVna1Mzetlk\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/_yWe6RfXRakoxhEk1-ozzg/89ptwjeBFP3SnrmoPRk5BqUeLtlhDjlR7fnOfiCPR9GHEMYHU-yiIikubT4I0z-0/2R6ZQWl2G0CxVijepsJGKT0BuSLMEVBmYRvwu6i35NQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/qZUc4gczarNtue6D4VYptg/w8GadHfQunnoYSvyLFk-KiEGvGPnZS5NMpEKj07sftpygR_e402Hmx7Q-_IBfXtL/Fj39ztMSWfvkGyfmmgq1lIgfUg9y1JcEu7Nvk_DYrE4\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/7uYqWLKifAEXjM8EzQTjbw/nA87e4Y_PxkzkDIkD0qBi98mJqcgAPFDEHIzmEtglUO0FZDwaY_P_R87pRuEHAlnMJnJxQjYGCLLH8Z_gEsMWw/hqqeDpkmJKE4i1hkqMq8GNnFjQvRsznaBnvBnOkjDsw\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/vkryG1DzjnWabAv_tehexQ/x0uCZitPOXkipRmtX97i2J541vRm-kqu7K1AuonFh8HVL2MSTAnQ8MQ1Z838oofpsQggWTI0g7EgKc9tl8Epqw/l9n6EipprK0yx52pVhdtfsafZShTO2pIdXBnIaQoqVE\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/VKMsP9p8g3GPoexym0qplA/RNON-5WBR_7tJjJrpqRgLLP4VQ68tIL8mnlMpQja0QHy0V0oPB3_-WX-QYIyI1Tbqol2HfvreIk5TBlCP4vsHg/pfodQ8TxVLLUGXSxi6TFcPNOqzWsZg005BrM5SJw-tE\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/V4bBZFsGnvFRPQYir3_rfQ/Y_oePC1vNHz9FnMzrO679f1J9fEQQaUBkMIdu45VtM5EUVztZ09R1f5RMJdQj2aoyCBmbiyjtwoLlUNCLTh9EQ/DMct3alEZujMTL0f90_yYpLQXqnuStkhClj0J6bdYvw\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/XboNJqc9zhXU7Cap6oG1UA/BIDPN9tsuhYu5TIBzXs6NVU2korlvmQmwDw0lQcR6eEmm8anJoNN-UC4cYeLNPb364H-QSa6G-sR9GpM1OZ4pFoNyzLDbC-2uokgf30POfhTsWq5c3KaFcjDrUUYkqoL42OiHohcXFHiaN6ZOxoXow/I3lzKVPkr5k6lJCF4IN9UCrJlg0sP-W5y8WguaBxpBo\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/L-T3mYiEvmGITaSXobOWTw/UXsQbMP8alRSkZFpqCoFWEiz3LfB-tFi4Tb7Jzgjk33SnP7kCh5KFfgtHlY_b1DWb9lRtlla_MqIur6qqCuejEn42jFo3m2xaCaM_iT7lUTYzMHTtlrZ1kT4Zf46iysy3tV2OoOZVe-GSQnbWtb5Cg/rvASU4aU2ugSvGccgUmKf9BMCNR8Av2a_Ak4ulwc-TQ\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/9HqGmYOXrDhnmgxWQEE9RA/pn6Pq6TtEla5Bp9YI3TFyxKE1Qh1Skya7UUrT0HXn5pNdhbv1XvnnBoT7IUbzPS_CGrRH-h4oWPQ8xnmZCURzPsE_X2O8-sOYWtpMoFJs3ZTmQjGtlsjgHu2mNigGOcjIl5NsB8o3as3Meyjk-m8zg/89miC-PxGE3ytGfK8_BjP2ndLogmfvfOh-5mf9baMgQ\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/tngRW1MSz3h-D3wKX0R5SA/JmxL1FQv0t32ek_qGxqbCnKlLuO7peTTwu6twKHI1zKVhDqZnlhjzB3ZZNPcrRxeDrkU98rxg1pkb8x67uHQ3f5rfEDEoixNRtS6w9xT1Udao6tdhP_kDSmglBo-FcaDB5GPGUVKQQlcPU0yPiDcYA/TP_O0i02b0DIYJ6_k_1Dd9brP33L2IUmIPP2RzNVRvY\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/HEk3_feeA8_bO3piLPIXdA/GCk6xgthTyXQYytkYd6aHml3pqSkN3gF4dK0q_ndNcDaicEIeAtRbiSDsVU2bOwAgRpqVe-hzwiaoJuaQNTAEP32DCYnTI_AnnnMRmm8ENcZcsm5Echr-maNV08lNDEDnjqgVMjmHSf7PWX0aM9mcosrLQQ_0mvqqfiFs5UPvcQKQ9Y6NFld_Dq8GHXnNe3W/4PUEeVQHwPC1ZLbfM4afElONlZiS2CWj1h6qAbXS-d0\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/TCyL4sXd4ugYNP8m23k4KA/PKsZt0EoQ8HLz5kVtmgd8OyykuxokBB1tOi4G9uqt1RshoEm-xKgMz2r7duYXQVrAcVZLGT9ERNrGhhpq6JTwWg3_Apus39AXF67VCZs2rnGU39weVYZHYtDmIHq4rJDWF3intTFCZuUDv-Jv189VXc_mzDMzoyUDwKx-_1g18qsP_TldInlQkD56ycgz3Fr/guulrTyNhUy5zfPT5zr-VYJoY2UHyRbrHrOhak7uH8Y\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/PQsGJkedSXWLw8LD9MOTlA/FiHpW-_OU8HMi2IJN1boOl2vRBsKa6XuVk9geOh_Ss71x0mQdjKUWaxoInoK6VrcdPBRiBhs7y0DX4N8puuTiWoK4AtXu3rrAohCFyE9X_XBPKYNqShlRTZZw8bf4NTDaSgfPBMKPxS1n1aj3wqknL7cVllpFuZuCcTdWPB2onHyYo0UMejlbI-324fgpx_E/axUkNP6SNqc8NI71RLTC3c0zr_xNM_6wZXSQjzkg3QI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678075200000/mTWbxq0YbxwYKpleqYjWHA/fxfTyvm6PHXHf6oj_TsdsARehMYiX3x_thy8uNtrb5c9mwGlVZXfTVRrZ4o3DQozdUUm-q6Lm06GwGOY8FV0-88DJ0AZsEefTRH6L3IRDF8BLCvoR-UPGZmAvb7O0LJiITjvg67k19ktz8hEgOnvcEToTg_2PtuGWSPeznehKinLSS3zZk9TSWVHhEpMhNpo/hZm9qrOaAU2hQdud0-bIeZCFsJecw7NzJ_2Gi9_1AOw\",\"width\":600,\"height\":600}}}]}}]}");
            
            string bodyText = "{\"maxRecords\":3,\"returnFieldsByFieldId\":true}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            Task<ListAllRecordsTestResponse> task = ListAllRecords(returnFieldsByFieldId: true, maxRecords: 3);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count > 0);
            var record = response.Records[0];                // We knew this is the record for "Al Held"
            string fieldIdOfFieldName = "fldSAUw6qVy9NzXzF";    // We also knew this is the field ID for "Al Held"
            string artistName = record.GetField<string>(fieldIdOfFieldName);
            Assert.IsTrue(artistName == "Al Held");
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TxAtApiClientCreateMultipleRecordstTest
        // Create all the records in the provided records[] in the same format as the one in AirtableApiListRecordsResponse.RecordList.
        // NOTE: Cannot add a new field when creating records
        // NOTE: The Attachment fields returned from Airtable is assigned an Id and have tons of other info so it cannot be used 'as is' 
        // for creating new records.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TxAtApiClientCreateMultipleRecordstTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec8aIgG19pQsmi4p\",\"createdTime\":\"2023-03-06T02:06:19.000Z\",\"fields\":{\"Name\":\"Willie\",\"Bio\":\"Bio for Willie\",\"Bank Name\":\"Key\"}},{\"id\":\"recM2wA5joXo1W9VE\",\"createdTime\":\"2023-03-06T02:06:19.000Z\",\"fields\":{\"Name\":\"Cassie\",\"Bio\":\"Bio for Cassie\",\"Bank Name\":\"Citi\"}},{\"id\":\"recTZVU1DLLmKKnWx\",\"createdTime\":\"2023-03-06T02:06:19.000Z\",\"fields\":{\"Name\":\"Ruby\",\"Attachments\":[{\"id\":\"attYxhYrHp2GAexkp\",\"url\":\"https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\"}],\"Bio\":\"Bio for Ruby\",\"Bank Name\":\"BOA\"}}]}");

            string bodyText = "{\"records\":[{\"fields\":{\"Attachments\":null,\"Name\":\"Willie\",\"Bio\":\"Bio for Willie\",\"Bank Name\":\"Key\"}},{\"fields\":{\"Attachments\":null,\"Name\":\"Cassie\",\"Bio\":\"Bio for Cassie\",\"Bank Name\":\"Citi\"}},{\"fields\":{\"Attachments\":[{\"url\":\"https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg\"}],\"Name\":\"Ruby\",\"Bio\":\"Bio for Ruby\",\"Bank Name\":\"BOA\"}}],\"typecast\":false}";
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            // Create Attachments list
            var attachmentList = new List<AirtableAttachment>();
            attachmentList.Add(new AirtableAttachment { Url = "https://upload.wikimedia.org/wikipedia/en/d/d1/Picasso_three_musicians_moma_2006.jpg"});

            const int arraySize = 3;
            AirtableRecord[] records = new AirtableRecord[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                records[i] = new AirtableRecord();
                records[i].Fields["Attachments"] = null;    // NOTE: The "Attachments" fields in the returned record list cannot be used in Create records?! (NN)
            }

            BuildRecordListWith3RecordsForTest(records);
            records[arraySize - 1].Fields["Attachments"] = attachmentList;

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.CreateMultipleRecords(TABLE_NAME, records);
            var response = await task;
            Assert.IsTrue(response.Success);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TyAtApiClientUpdateMultipleRecordstest
        // Update multiple records in one single operation using the batch Update API.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TyAtApiClientUpdateMultipleRecordstest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recLLKvNrSlcKQ7Jj\",\"createdTime\":\"2023-03-06T06:10:33.000Z\",\"fields\":{\"Name\":\"Ruby\",\"Attachments\":[{\"id\":\"attvDQGU6bILnhOUA\",\"width\":335,\"height\":296,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/-v2F5YFdE1u_LEvLCtPiNA/4Xtxrr5yOeO_kVN3iHyg6mORx-DLk11Wuk9K5ZZtbaQHqek10baYb0rMEYgWOGBYkx0pHdaJ_Ec_DdFfle2boOtzZZaGWrV7uWtTS5VEqiXFXKcORJtBK3jXh4PzjFD5/Fg-X-WlKwhHzudX6tSb91pT37rj7c_irswAKwP9PQgQ\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\",\"size\":17604,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/To5bPlvPGBNdnAXlHASFPA/7HE9ZUsI9GfurokALradAWZITYrB9B3l3pMZrskITavf8KWdMUFfRzbzi53PYRVvVeilbv58Tpw0oZnF61Ms3A/4xiqcEm1Qr7TV6Z5me0FrvYPpdf8MK4gYTRxaFunoIU\",\"width\":41,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/s93KdCPe3G1ycE2qrm3c6Q/2B7bPu-qXZTUiu8WwxThHFQawG9lBgBt2utR8sc4kXlb3tsCk39WttvkYCiaTdTYJD4ORC0UDr5Sae75v2-Bow/9PQXjiK7WK3dM4fGQNkhDlJuxjVd2DaPyYogn7p5884\",\"width\":335,\"height\":296},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/Vqqe_j-5cRZg4ePGmE6sRw/rRPmBxoGMYFXyU3JRoI66SwkcSfW9wCoJjtkUp9XRamBxECuXlW7vnrr0H4KjqTzhd_4QKbBSte43T6WXeoSKA/pcny7uMe5U2VwB12pvLJWrMxYYW6FmP7pbiLoTwJmf4\",\"width\":3000,\"height\":3000}}}],\"Bio\":\"Bio for Ruby\",\"Bank Name\":\"BOA\"}},{\"id\":\"recXCXkVOHZP7lw3u\",\"createdTime\":\"2023-03-06T06:10:33.000Z\",\"fields\":{\"Name\":\"Cassie\",\"Bio\":\"Bio for Cassie\",\"Bank Name\":\"Citi\"}},{\"id\":\"rech3uNV4AaTbYo42\",\"createdTime\":\"2023-03-06T06:10:33.000Z\",\"fields\":{\"Name\":\"Willie\",\"Bio\":\"Bio for Willie\",\"Bank Name\":\"Key\"}}]}");

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                null);

            //?filterByFormula = NOT(% 7bBank + Name % 7d +% 3d +% 27 % 27)
            Task<AirtableRecord[]> task = GetRecordsWithFormula();

            var records = await task;
            Assert.IsNotNull(records);
            foreach (var record in records)
            {
                string bankName = record.Fields["Bank Name"].ToString();
                record.Fields["Bank Name"] = bankName + "Updated";
            }

            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recLLKvNrSlcKQ7Jj\",\"createdTime\":\"2023-03-06T06:10:33.000Z\",\"fields\":{\"Name\":\"Ruby\",\"Attachments\":[{\"id\":\"attvDQGU6bILnhOUA\",\"width\":335,\"height\":296,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/-v2F5YFdE1u_LEvLCtPiNA/4Xtxrr5yOeO_kVN3iHyg6mORx-DLk11Wuk9K5ZZtbaQHqek10baYb0rMEYgWOGBYkx0pHdaJ_Ec_DdFfle2boOtzZZaGWrV7uWtTS5VEqiXFXKcORJtBK3jXh4PzjFD5/Fg-X-WlKwhHzudX6tSb91pT37rj7c_irswAKwP9PQgQ\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\",\"size\":17604,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/To5bPlvPGBNdnAXlHASFPA/7HE9ZUsI9GfurokALradAWZITYrB9B3l3pMZrskITavf8KWdMUFfRzbzi53PYRVvVeilbv58Tpw0oZnF61Ms3A/4xiqcEm1Qr7TV6Z5me0FrvYPpdf8MK4gYTRxaFunoIU\",\"width\":41,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/s93KdCPe3G1ycE2qrm3c6Q/2B7bPu-qXZTUiu8WwxThHFQawG9lBgBt2utR8sc4kXlb3tsCk39WttvkYCiaTdTYJD4ORC0UDr5Sae75v2-Bow/9PQXjiK7WK3dM4fGQNkhDlJuxjVd2DaPyYogn7p5884\",\"width\":335,\"height\":296},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/Vqqe_j-5cRZg4ePGmE6sRw/rRPmBxoGMYFXyU3JRoI66SwkcSfW9wCoJjtkUp9XRamBxECuXlW7vnrr0H4KjqTzhd_4QKbBSte43T6WXeoSKA/pcny7uMe5U2VwB12pvLJWrMxYYW6FmP7pbiLoTwJmf4\",\"width\":3000,\"height\":3000}}}],\"Bio\":\"Bio for Ruby\",\"Bank Name\":\"BOAUpdated\"}},{\"id\":\"recXCXkVOHZP7lw3u\",\"createdTime\":\"2023-03-06T06:10:33.000Z\",\"fields\":{\"Name\":\"Cassie\",\"Bio\":\"Bio for Cassie\",\"Bank Name\":\"CitiUpdated\"}},{\"id\":\"rech3uNV4AaTbYo42\",\"createdTime\":\"2023-03-06T06:10:33.000Z\",\"fields\":{\"Name\":\"Willie\",\"Bio\":\"Bio for Willie\",\"Bank Name\":\"KeyUpdated\"}}]}");

            string bodyText = "{\"returnFieldsByFieldId\":false,\"typecast\":false,\"records\":[{\"id\":\"recLLKvNrSlcKQ7Jj\",\"fields\":{\"Name\":\"Ruby\",\"Attachments\":[{\"id\":\"attvDQGU6bILnhOUA\",\"width\":335,\"height\":296,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/-v2F5YFdE1u_LEvLCtPiNA/4Xtxrr5yOeO_kVN3iHyg6mORx-DLk11Wuk9K5ZZtbaQHqek10baYb0rMEYgWOGBYkx0pHdaJ_Ec_DdFfle2boOtzZZaGWrV7uWtTS5VEqiXFXKcORJtBK3jXh4PzjFD5/Fg-X-WlKwhHzudX6tSb91pT37rj7c_irswAKwP9PQgQ\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\",\"size\":17604,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/To5bPlvPGBNdnAXlHASFPA/7HE9ZUsI9GfurokALradAWZITYrB9B3l3pMZrskITavf8KWdMUFfRzbzi53PYRVvVeilbv58Tpw0oZnF61Ms3A/4xiqcEm1Qr7TV6Z5me0FrvYPpdf8MK4gYTRxaFunoIU\",\"width\":41,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/s93KdCPe3G1ycE2qrm3c6Q/2B7bPu-qXZTUiu8WwxThHFQawG9lBgBt2utR8sc4kXlb3tsCk39WttvkYCiaTdTYJD4ORC0UDr5Sae75v2-Bow/9PQXjiK7WK3dM4fGQNkhDlJuxjVd2DaPyYogn7p5884\",\"width\":335,\"height\":296},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/Vqqe_j-5cRZg4ePGmE6sRw/rRPmBxoGMYFXyU3JRoI66SwkcSfW9wCoJjtkUp9XRamBxECuXlW7vnrr0H4KjqTzhd_4QKbBSte43T6WXeoSKA/pcny7uMe5U2VwB12pvLJWrMxYYW6FmP7pbiLoTwJmf4\",\"width\":3000,\"height\":3000}}}],\"Bio\":\"Bio for Ruby\",\"Bank Name\":\"BOAUpdated\"}},{\"id\":\"recXCXkVOHZP7lw3u\",\"fields\":{\"Name\":\"Cassie\",\"Bio\":\"Bio for Cassie\",\"Bank Name\":\"CitiUpdated\"}},{\"id\":\"rech3uNV4AaTbYo42\",\"fields\":{\"Name\":\"Willie\",\"Bio\":\"Bio for Willie\",\"Bank Name\":\"KeyUpdated\"}}]}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/",
                    new HttpMethod("Patch"),
                    fakeResponse,
                    bodyText);

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task2 = airtableBase.UpdateMultipleRecords(TABLE_NAME, records);
            var response2 = await task2;
            Assert.IsTrue(response2.Success);
            foreach (var record in response2.Records)
            {
                Assert.IsTrue(record.Fields["Bank Name"].ToString().Contains("Updated"));
                Assert.IsNotNull(record.Fields["Bio"]);         // Update operation should be non-destructive
            }
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzAtApiClientReplaceMultipleRecordsTest
        // Replace multiple records in one single operation using the batch Replace API
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzAtApiClientReplaceMultipleRecordsTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recLLKvNrSlcKQ7Jj\",\"createdTime\":\"2023-03-06T06:10:33.000Z\",\"fields\":{\"Bio\":\"Bio for Ruby\",\"Name\":\"Ruby\",\"Bank Name\":\"BOAUpdated\",\"Attachments\":[{\"id\":\"attvDQGU6bILnhOUA\",\"width\":335,\"height\":296,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/-v2F5YFdE1u_LEvLCtPiNA/4Xtxrr5yOeO_kVN3iHyg6mORx-DLk11Wuk9K5ZZtbaQHqek10baYb0rMEYgWOGBYkx0pHdaJ_Ec_DdFfle2boOtzZZaGWrV7uWtTS5VEqiXFXKcORJtBK3jXh4PzjFD5/Fg-X-WlKwhHzudX6tSb91pT37rj7c_irswAKwP9PQgQ\",\"filename\":\"Picasso_three_musicians_moma_2006.jpg\",\"size\":17604,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/To5bPlvPGBNdnAXlHASFPA/7HE9ZUsI9GfurokALradAWZITYrB9B3l3pMZrskITavf8KWdMUFfRzbzi53PYRVvVeilbv58Tpw0oZnF61Ms3A/4xiqcEm1Qr7TV6Z5me0FrvYPpdf8MK4gYTRxaFunoIU\",\"width\":41,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/s93KdCPe3G1ycE2qrm3c6Q/2B7bPu-qXZTUiu8WwxThHFQawG9lBgBt2utR8sc4kXlb3tsCk39WttvkYCiaTdTYJD4ORC0UDr5Sae75v2-Bow/9PQXjiK7WK3dM4fGQNkhDlJuxjVd2DaPyYogn7p5884\",\"width\":335,\"height\":296},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678096800000/Vqqe_j-5cRZg4ePGmE6sRw/rRPmBxoGMYFXyU3JRoI66SwkcSfW9wCoJjtkUp9XRamBxECuXlW7vnrr0H4KjqTzhd_4QKbBSte43T6WXeoSKA/pcny7uMe5U2VwB12pvLJWrMxYYW6FmP7pbiLoTwJmf4\",\"width\":3000,\"height\":3000}}}]}},{\"id\":\"recXCXkVOHZP7lw3u\",\"createdTime\":\"2023-03-06T06:10:33.000Z\",\"fields\":{\"Bio\":\"Bio for Cassie\",\"Name\":\"Cassie\",\"Bank Name\":\"CitiUpdated\"}},{\"id\":\"rech3uNV4AaTbYo42\",\"createdTime\":\"2023-03-06T06:10:33.000Z\",\"fields\":{\"Bio\":\"Bio for Willie\",\"Name\":\"Willie\",\"Bank Name\":\"KeyUpdated\"}}]}");
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                null);

            Task<AirtableRecord[]> task = GetRecordsWithFormula();
            var records = await task;
            Assert.IsNotNull(records);
            foreach (var record in records)
            {
                string bankName = record.Fields["Bank Name"].ToString();
                record.Fields["Bank Name"] = bankName + "Replaced";
                record.Fields.Remove("Bio");
            }

            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recgVehpCSGFpy0mB\",\"createdTime\":\"2022-07-13T19:24:28.000Z\",\"fields\":{\"Name\":\"Willie\",\"Bank Name\":\"KeyUpdatedReplaced\"}},{\"id\":\"reclSZBWrMoWuaMeW\",\"createdTime\":\"2022-07-13T19:24:29.000Z\",\"fields\":{\"Name\":\"Ruby\",\"Bank Name\":\"BOAUpdatedReplaced\"}},{\"id\":\"recrTBCcUVt3ufXEH\",\"createdTime\":\"2022-07-13T19:24:28.000Z\",\"fields\":{\"Name\":\"Cassie\",\"Bank Name\":\"CitiUpdatedReplaced\"}}]}");

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/",
                HttpMethod.Put,
                fakeResponse);

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task2 = airtableBase.ReplaceMultipleRecords(TABLE_NAME, records);
            var response2 = await task2;
            Assert.IsTrue(response2.Success);
            foreach (var record in response2.Records)
            {
                Assert.IsTrue(record.Fields["Bank Name"].ToString().Contains("Replaced"));
                Assert.IsFalse(record.Fields.ContainsKey("Bio"));            // Replace operation is destructive. We should no longer find the 'Bio' field.
            }
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzAtApiClientReplaceMultipleRecordsTest_PerformUpsert
        // Replace all the records in the provided records[] in the same format as the one in AirtableApiListRecordsResponse.RecordList.
        // PerformUpsert is included to allow the upsert operation.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzAtApiClientReplaceMultipleRecordsTest_UsingPerformUpsert()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"reciLHL1j9iKOAmAd\",\"createdTime\":\"2023-03-06T20:26:12.000Z\",\"fields\":{\"Name\":\"Pierre-Auguste Renoir\",\"Bio\":\"Renoir was a celebrator of beauty and especially feminine sensuality\"}},{\"id\":\"reckuK4megeU6gVot\",\"createdTime\":\"2023-03-06T20:26:12.000Z\",\"fields\":{\"Name\":\"Edouard Manet\",\"Bio\":\"Manet was a French modernist painter. He was one of the first 19th-century artists to paint modern life, as well as a pivotal figure in the transition from Realism to Impressionism\"}}]}");            
            
            string bodyText = "{\"records\":[{\"fields\":{\"Name\":\"Pierre-Auguste Renoir\",\"Bio\":\"Renoir was a celebrator of beauty and especially feminine sensuality\"}},{\"fields\":{\"Name\":\"Edouard Manet\",\"Bio\":\"Manet was a French modernist painter. He was one of the first 19th-century artists to paint modern life, as well as a pivotal figure in the transition from Realism to Impressionism\"}}],\"typecast\":true}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/",
                    HttpMethod.Post,
                    fakeResponse,
                bodyText);

            Fields[] fields = new Fields[2];
            fields[0] = new Fields();
            fields[0].AddField("Name", "Pierre-Auguste Renoir");
            fields[0].AddField("Bio", "Renoir was a celebrator of beauty and especially feminine sensuality");

            fields[1] = new Fields();
            fields[1].AddField("Name", "Edouard Manet");
            fields[1].AddField("Bio", "Manet was a French modernist painter. He was one of the first 19th-century artists to paint modern life, as well as a pivotal figure in the transition from Realism to Impressionism");

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.CreateMultipleRecords(TABLE_NAME, fields, true);
            var response = await task;
            string renoirId = null;
            string manetId = null;
            foreach (var record in response.Records)
            {
                string name = record.GetField<string>("Name");
                if (name == "Pierre-Auguste Renoir")
                {
                    renoirId = record.Id;
                }
                else
                {
                    manetId = record.Id;
                }
            }

            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"reciLHL1j9iKOAmAd\",\"createdTime\":\"2023-03-06T20:26:12.000Z\",\"fields\":{\"Name\":\"Pierre-Auguste Renoir\",\"Bank Name\":\"Banque Republique de France\"}},{\"id\":\"reckuK4megeU6gVot\",\"createdTime\":\"2023-03-06T20:26:12.000Z\",\"fields\":{\"Bio\":\"He is as good as his father\",\"Name\":\"Renoir-son\"}},{\"id\":\"recGKfzWzSmSlqenh\",\"createdTime\":\"2023-03-06T21:02:48.000Z\",\"fields\":{\"Name\":\"Edgar Degas\",\"Bio\":\"He is an honest artist.\",\"On Display?\":true,\"Bank Name\":\"Chase Bank\"}}],\"updatedRecords\":[\"reciLHL1j9iKOAmAd\",\"reckuK4megeU6gVot\"],\"createdRecords\":[\"recGKfzWzSmSlqenh\"]}");

            bodyText = "{\"performUpsert\":{\"fieldsToMergeOn\":[\"Name\",\"Bio\",\"Bank Name\"]},\"returnFieldsByFieldId\":false,\"typecast\":false,\"records\":[{\"id\":\"reciLHL1j9iKOAmAd\",\"fields\":{\"Name\":\"Pierre-Auguste Renoir\",\"Bank Name\":\"Banque Republique de France\"}},{\"id\":\"reckuK4megeU6gVot\",\"fields\":{\"Name\":\"Renoir-son\",\"Bio\":\"He is as good as his father\"}},{\"fields\":{\"Name\":\"Edgar Degas\",\"On Display?\":true,\"Bank Name\":\"Chase Bank\",\"Bio\":\"He is an honest artist.\"}}]}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/",
                HttpMethod.Put,
                fakeResponse,
                bodyText);

            IdFields[] idFields = new IdFields[3];
            idFields[0] = new IdFields(renoirId);
            idFields[0].AddField("Name", "Pierre-Auguste Renoir");
            idFields[0].AddField("Bank Name", "Banque Republique de France");

            idFields[1] = new IdFields(manetId);            // This record should replace the Manet record.
            idFields[1].AddField("Name", "Renoir-son");
            idFields[1].AddField("Bio", "He is as good as his father");

            idFields[2] = new IdFields();                   // no ID is specified. This record should be created.
            idFields[2].AddField("Name", "Edgar Degas");
            idFields[2].AddField("On Display?", true);
            idFields[2].AddField("Bank Name", "Chase Bank");
            idFields[2].AddField("Bio", "He is an honest artist."); // If this line is missing -> Error: "Record must include columns to merge on."
                                                                    // becaukse 'Bio' is specified in FieldsToMergeOn and this record has to be created
                                                                    // so we need to make sure such record does not exist already (but this does not sound
                                                                    // logical to me. ==> check with Emmett.


            PerformUpsert performUpsert = new PerformUpsert();
            performUpsert.FieldsToMergeOn = new string[3];
            performUpsert.FieldsToMergeOn[0] = "Name";
            performUpsert.FieldsToMergeOn[1] = "Bio";
            performUpsert.FieldsToMergeOn[2] = "Bank Name";     // If this line is added then the record to be created must have this column.
                                                                // Otherwise -> Error: Record must include columns to merge on. 

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task2 = airtableBase.ReplaceMultipleRecords(TABLE_NAME, idFields, performUpsert: performUpsert);
            var response2 = await task2;
            Assert.IsTrue(response2.Success);

        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzAtApiClientUpdateMultipleRecordsTest_UsingPerformUpsert
        // Replace all the records in the provided records[] in the same format as the one in AirtableApiListRecordsResponse.RecordList.
        // PerformUpsert is included to allow the upsert operation.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzAtApiClientUpdateMultipleRecordsTest_UsingPerformUpsert()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"reco56hjg6BNtZdSC\",\"createdTime\":\"2023-03-06T23:07:22.000Z\",\"fields\":{\"Name\":\"Pierre-Auguste Renoir\",\"Bio\":\"Renoir was a celebrator of beauty and especially feminine sensuality\"}},{\"id\":\"recMuuOoal4HXEQg1\",\"createdTime\":\"2023-03-06T23:07:22.000Z\",\"fields\":{\"Name\":\"Edouard Manet\",\"Bio\":\"Manet was a French modernist painter. He was one of the first 19th-century artists to paint modern life, as well as a pivotal figure in the transition from Realism to Impressionism\"}}]}");

            string bodyText = "{\"records\":[{\"fields\":{\"Name\":\"Pierre-Auguste Renoir\",\"Bio\":\"Renoir was a celebrator of beauty and especially feminine sensuality\"}},{\"fields\":{\"Name\":\"Edouard Manet\",\"Bio\":\"Manet was a French modernist painter. He was one of the first 19th-century artists to paint modern life, as well as a pivotal figure in the transition from Realism to Impressionism\"}}],\"typecast\":true}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/",
                    HttpMethod.Post,
                    fakeResponse,
                bodyText);

            Fields[] fields = new Fields[2];
            fields[0] = new Fields();
            fields[0].AddField("Name", "Pierre-Auguste Renoir");
            fields[0].AddField("Bio", "Renoir was a celebrator of beauty and especially feminine sensuality");

            fields[1] = new Fields();
            fields[1].AddField("Name", "Edouard Manet");
            fields[1].AddField("Bio", "Manet was a French modernist painter. He was one of the first 19th-century artists to paint modern life, as well as a pivotal figure in the transition from Realism to Impressionism");

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.CreateMultipleRecords(TABLE_NAME, fields, true);
            var response = await task;

            Assert.IsTrue(response.Success);

            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recMuuOoal4HXEQg1\",\"createdTime\":\"2023-03-06T23:07:22.000Z\",\"fields\":{\"Name\":\"Edouard Manet\",\"Bio\":\"Manet was a French modernist painter. He was one of the first 19th-century artists to paint modern life, as well as a pivotal figure in the transition from Realism to Impressionism\",\"Bank Name\":\"BOA\"}},{\"id\":\"recXmk1FbcdyIMSz1\",\"createdTime\":\"2023-03-06T23:14:21.000Z\",\"fields\":{\"Name\":\"Edgar Degas\",\"On Display?\":true}}],\"updatedRecords\":[\"recMuuOoal4HXEQg1\"],\"createdRecords\":[\"recXmk1FbcdyIMSz1\"]}");

            bodyText = "{\"performUpsert\":{\"fieldsToMergeOn\":[\"Name\"]},\"returnFieldsByFieldId\":false,\"typecast\":false,\"records\":[{\"fields\":{\"Name\":\"Edgar Degas\",\"On Display?\":true}},{\"fields\":{\"Name\":\"Edouard Manet\",\"Bank Name\":\"BOA\"}}]}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/",
                    new HttpMethod("Patch"),
                    fakeResponse,
                    bodyText);

            PerformUpsert performUpsert = new PerformUpsert();
            performUpsert.FieldsToMergeOn = new string[1];
            performUpsert.FieldsToMergeOn[0] = "Name";
            // Create fields array
            IdFields[] idFields = new IdFields[2];
            idFields[0] = new IdFields();                   // Record ID is optional when performUpsert is included.
            idFields[0].AddField("Name", "Edgar Degas");
            idFields[0].AddField("On Display?", true);

            idFields[1] = new IdFields();
            idFields[1].AddField("Name", "Edouard Manet");
            idFields[1].AddField("Bank Name", "BOA");

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task2 = airtableBase.UpdateMultipleRecords(TABLE_NAME, idFields, performUpsert: performUpsert);
            var response2 = await task2;
            Assert.IsTrue(response2.Success);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzAtApiClientUpdateMultipleRecordsTestWithError_UsingPerformUpsert
        // PerformUpsert is not included and no record ID is provided.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzAtApiClientUpdateMultipleRecordsTestWithError_UsingPerformUpsert()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"recVKMzJoCEJlKosB\",\"createdTime\":\"2023-03-06T23:34:36.000Z\",\"fields\":{\"Name\":\"Edouard Manet\",\"Bank Name\":\"BOA\"}}]}");

            string bodyText = "{\"records\":[{\"fields\":{\"Name\":\"Edouard Manet\",\"Bank Name\":\"BOA\"}}],\"typecast\":true}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/",
                    HttpMethod.Post,
                    fakeResponse,
                    bodyText);

            Fields[] fields = new Fields[1];
            fields[0] = new Fields();
            fields[0].AddField("Name", "Edouard Manet");
            fields[0].AddField("Bank Name", "BOA");

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.CreateMultipleRecords(TABLE_NAME, fields, true);
            var response = await task;
            Assert.IsTrue(response.Success);

            fakeResponse.Content = new StringContent
                ("");   // dummy content

            bodyText = "{\"returnFieldsByFieldId\":false,\"typecast\":false,\"records\":[{\"fields\":{\"Name\":\"Edouard Manet\",\"Bio\":\"Manet was a French modernist painter. He was one of the first 19th-century artists to paint modern life, as well as a pivotal figure in the transition from Realism to Impressionism\"}}]}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/",
                    HttpMethod.Post,
                    fakeResponse,
                    bodyText);

            // Create fields array
            IdFields[] idFields = new IdFields[1];
            idFields[0] = new IdFields();
            idFields[0].AddField("Name", "Edouard Manet");
            idFields[0].AddField("Bio", "Manet was a French modernist painter. He was one of the first 19th-century artists to paint modern life, as well as a pivotal figure in the transition from Realism to Impressionism");

            Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task2 = airtableBase.UpdateMultipleRecords(TABLE_NAME, idFields);
            var response2 = await task2;
            Assert.IsFalse(response2.Success);      // Operation should fail because record ID is not specified and performUpset is not included
            if (response2.AirtableApiError != null)
            {
                if (response2.AirtableApiError.ErrorMessage != null)
                {
                    Console.WriteLine("ErrorMessage is {0}", response2.AirtableApiError.ErrorMessage);
                }
                if (response2.AirtableApiError.DetailedErrorMessage != null)
                {
                    Console.WriteLine("AirtableApiError.DetailedErrorMessage is {0}", response2.AirtableApiError.DetailedErrorMessage);
                }
            }
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzBbAtApiClientListRecordsCellFormatTest
        // List records
        // Returned records do not include any fields with "empty" values, e.g. "", [], or false.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzBbAtApiClientListRecordsCellFormatTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":\"checked\",\"Genre\":\"American Abstract Expressionism, Color Field\",\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29 11:00am\",\"Collection\":\"Transcontinental Legacy\",\"Attachments\":\"Quattro_Centric_XIV.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/UzYqZUTZvOu09lcQlMMP_w/r5EKKp5lXivxIs3-PP1iUoQBCk4V9nkNEHqzCG9qts2y0I4B4nIF5xtJGakQteI7PIwIi8_6jm4wHel-vlOlgbGZcZ0oRHaqLGlJrcESNWXCXCRMUOBfoMAZXMPIKhFF/JM1r3fkqBe_gvJ40ouBYot4LGITOK5rPhXGJDAdlRSk), Roberta's_Trip.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/hJbCNPHwUo_SlsOMawZN-Q/M9iaBTBnOc2HJ8fH3FAtGTR3H13b0cjU9CKr-cBn60KySEJWOJwnFmhBmYVv_lIQHtEJ9GI-8RgCLsU_C66UUoc1ARKsQTkNWbKJWuEAImB3akbyecFyBjs8S9yIzDbC/Anv8ISSo1GgOJuWYbyLpJbAatOZ9Qh703GXvYEIpYls), Bruges_III.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/7qRcHLSgwgwRHfCDDB2KAA/n-SuixaHLccoJ1jwO7A09eOucwD6XM1Ncf1aeggp_DVCE02na_Cf9qith-laWoO3My59vH-OTQRncRd19O9RdlArbJUkyFP2muSq4lVS-_A/V4f_wTh1J1Y5R5BnGWqnrunRj1dsBSrcU-auwgZRI1E), Vorcex_II.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/X45DUaNO900Vlde4F8-B0Q/RxjuKrdGvrKq2AhquJaIHWRELbXxi25EV7FKD5ITiNu6mVqZa2Se0wJhHBUXGSBz6LH9d9ZHyDGLYReEkHeujS3cpQNUf-5S38Yb0kMzqDs/FWsolb9uFsXa8JpIa1KH2iaJ9cOuok6a4WsugokvBEk)\"} },{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":\"checked\",\"Genre\":\"Abstract Expressionism, Modern art\",\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\n\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":\"Transcontinental Legacy\",\"Attachments\":\"Master-Bill.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/hmb2Vu_a3jJ_giVUG0R5OA/HSsxbm4kkWPK7jjAQHF5YbGNJySsq18P2qagbZ3WVgxNnuUCGWVb0O9Wiy3oFxAZwys-cx_r2mBahHD41P5ZjBZPnBTyqu2P6cGusD5tlwM/q5nrv-aIeZsP0Jl_MI29Vw-fL15ch_oSK5p4EuvCiRY), The_Liver_Is_The_Cock's_Comb.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/eKakt4XvvQ6-DxiQZtK0Rg/1RkwcQPQ2ZXsthjpaW2U7gEv6WzKnmkh4ESwqdmM_kkt2rPXT66YCE1sNLFkXNXU7_msW5DmleFfsVRYVYlYgkbx6SKR4Ymc_lIs1QXFpQ_BaEkR5aSXNITnTyTqyt_2/flz50OH-FyCRr7lqAgyb4QiIHmHFHYRlCCGXnRYYv8k), Garden-in-Sochi-1941.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/07yl-qs7mavAbMDYbV1JwA/kwyO4LjVX48N8kUi_d9OspOHukd7Yr949KKUDO8eeoURFcW3Plj3jz7PfCsO7LH1o-DSVt-2d2cX2Bk8Uugb-muMG-dcTn0kN4wZqvju2r3oNUkQKByRVs_y7HHydot5/VjoXV7Yh-5_VjN7Wqr7QjNf2AaSJxsQHICuwuG4RA9k)\"} },{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":\"Post-minimalism, Color Field\",\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":\"Color Field\",\"Attachments\":\"blue+light.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/HXI5h8quR9iB-RQK3DxuVg/XaXzFswVJXL_nzLWFPdg-myB8rzz5p4DJH7skjcUakx358l9TZfjWIWWDZMTEp__tI4KZibsZqlkbrzsmo1S_ZC6Au0SNsKqBoY8ztUNf-c/GScVHi9bsD_4DAla3Hy-D2f9aQUqKEuJHMtcIClUGwU), miya_ando_sui_getsu_ka_grid-copy.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/G9vPsw6SBQiAZ7E0WsCl6g/IMhVZQWFk73OD-v_2DeH6dTEs8zAGitnqqRLmcY-0iy0HKemyMZDvwVCm6n2mpgduOWq5NVEwB1Dq0ZIrH_m4Tnuphc1HXSpewIiKzOX7K_zJNevrR58QX7KRvrsFpP2KgUoVWL-OW4jmUGeLtF-Lg/4jOChKlCupTV4kO6DSmUzMD-dhtRnC3UbW78anKjFT4), miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/4W0YyML4mSLIpY9wrO5d2g/AaB4F_BA3AR_r6tN5unL919Qw0CaxW8n3KSLKXN1hDekbpX-JxL9DFWiec04C-V3JA9LVHXKgMXFOOc0INwUXpolG01pXQp29a36cksQc_CG4mIb79TkFtVeFRHWE2iX-p0Lin95Fv3vmXYSyBYozViSMj41debB2pky5xZ7YdevFsox4xanR8kuf8lGIIdF/naG1LnseYUR6GMGtFa_e3xro0j8YKwCJaWN78aV9SQU), miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg (https://v5.airtableusercontent.com/v2/23/23/1701410400000/D-UrbwGi6XtnZ8GeNQ-bFg/sqn-_CuYzs_FWA1TFTW9-UxdCLIFB9FMER4p6BY_vcvJfM8F4-VzXb4d-ZUa_QXp-hpIdB5Yuzvd6BsdVSPEJSmS2b9AjYCDdf8LpNVnkRpKZV8LoW27RR2SqgAocy-q3lXkGD2F7roKJD3-yN9KAahihtfnVusRHMHDebcou4nHtZC7OoQxXaUTjv7VvB4jT8ysTnWUOap2JTvc840N7wMCW-O0u_NXAtLykmEl8aI/ExUBfjr7vV4NOxeatVbzXo4f3gS9i4GxVAs2HThR5-8)\"}}]}");

            string bodyText = "{\"maxRecords\":3,\"cellFormat\":\"string\",\"timeZone\":\"Asia/Ho_Chi_Minh\",\"userLocale\":\"fr-ca\",\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            Task<ListAllRecordsTestResponse> task = ListAllRecords(cellFormat: "string", timeZone: "Asia/Ho_Chi_Minh", userLocale: "fr-ca", maxRecords: 3);
            var response = await task;
            Assert.IsTrue(response.Success);

            ////////////////////////////////////////////////////
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/cLzPASbaf-nhf0UfjMacog/5ZzgWwVWbZnjCK9e7QPrcwnasstesEAxOOSpdVHw-NCUmnMnhP1rAeKoiTKe8QUP/XIXlu3ElxqKk51hWBuABlWSjBBl6T7ksD_eEwggKJ3Y\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/tlqMe4shNaYUDtc5S_Y8Tg/_U6I7kyOn6rcJxu7jFiY8octZRECAJeBFB1eeT4l50bS8eruSJPUc0qYHNgpA4yGhp7FL0A9dMyH8hoQDejMTg/oDAROzKuQ2ABMUndA98XWOPELNtYESfi2uSEHZoZu0w\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/HDgWDJvJNdLzMzYkjqbW2A/nPthWm8tI7TSOPRhUf8GuPEXtMDAyBpFpvrQyC3y2E5Mpg6S-CV1sanSjyRAn-TETkBpJhV64pyeGEzkz7Mi3w/TJrinb5k_HQCEpH7PmEokol320pyCHvDqsA7FxsBfDQ\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/wP1BHoK7YrIXd7OnJdpnSw/OE_Q71YL8tBB7ZjCbk_j7IKJu7EPzOHLD3XFcyan4ynkHSxtJz5kKbFtMrz_zTLX9unz1xRqHRD0Hw6KtrduVg/7ksoo9_nDLyG9YZzEMU2-EUipHqnpERY2RRts3umONA\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/6uSGZ3L7CwDmn81y4fV47A/HxMfXfYFw-q8F5wMfBH6E6F07GBwfQA5rRZ5IxaoPaBU0-woASJb1HyObbOdnkbW/xY0We1wUPocACDziCd7dMJzY-laE37M1vLCA-yp8Td8\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/N77JSAtZkMg9YOvf19N4MQ/tDpZeLtIefltlndON_vFBFB3nbHO2mVD_n2MQoiBmsmbKYWrV36atwzTvzpIJHL9/oZt34Izfl2t94TtpapfEoIUFpSuF21BPXNUDcH7qC7I\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/vREz0T6TCXKeleSBkwtj3w/6ZX5UYhQMM7O1v_d-sbofbmg6mIzp68gv_pBMr1y1zI2Vl0UfiR6_wGhlcRf7x_p/sPxM5MKHFMiBPTAaIU_B79BqOdsvVpWdU5GURs75iuQ\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/EgS3CLgxyCyNrgH59dAMGQ/DKw_sg0QnZmmRKKTDsA_p_bcBwn-uIXXZuJ5RtgtqnmYr_PYu0GTehOkPxrxD4Jd/9cgpm6cPoqKZOT_Rki6wVhCqCKjK5imOpZ2MW9UFttQ\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/tpgBurj4DgwM4D4oyMu8fw/33zbSb-H7s_rhimfgPYOB_WtMWrD4my1jSVBKYyr9f0fjUpigvljfon7Fk5lh0vQ/F39PORYT5jJFnaoQgu7H7PoFgaw7b8gi5UU5bi-AoCA\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/1FlTXYK1zSIgT1ed5_0_Ow/-N6BTdbnFwDVWDld-GEADenidwgudlq1fL0RFNvw8iHS3RiyTdVgRxrLqv35XzcT/g4yFqPj0vj48YKsgKy7jveWEPXPbDCrqIfv5xLxr_rw\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/cJZTwcDd8qionKdANQPquw/6pPDQki9cm8nXHTvItojQaEWpLKZMSC7IA9KFqp6NRO2KCR_9TyxIv5GjedZJGXV/kab-AoRLBj7jSch2eCzMN9Yy0-oekkl0DKZQ7ehAmTk\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/6DjhD_JAywdonP8IOXSEXg/BZARvy-VrGndMjkPS8FLKCWZKEm-iO9o79iKey2_pXCrpgBoPfUMd_-cihSTuLbW/JYdAXuTnCa9un3-lZ0gvPJqumdfxw0KgAPC_7GRBtb4\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/5pBQrmc_Va3eZ2qZaxflBA/eUHxbkdQnKveKniHv3wCx9H2h4If3-ExlGgOt_TIMfpkaw7Lpm7RtvraY8mXb1ts/gQs8RdaTIE4sWfsG9elUutFUico-SP6_unPver9nx54\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/u7o2Mj2c2E_J-f8QJd3VxQ/SUYEFrt98j2AwEORNqpKjsZ5VoN637Le-tDkYRA1pjo55SI9_1rfRBg2wHTW81OL/R3_h7rJiqbcuAYIWN7GWUWc4Zp9B56-u73INzSHpcfI\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/UXPjRZQDgfAeVOOajonxGQ/1LnzZRAy5htjlLsHIApyG3HSokxQqkilFlUW_0HXgvK_G5RpCRfm_gNDmvpFbq3U/qiIhB0Y0if5PwS9eUWRMlyyHjMGQ1dvBp_5qtQHClow\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/J5L7T7pOiuX1F7ptYR5A_w/r72EOG3S6Z7EJcymnR7B7jcUfcCWMwD7oJ1aTpYYMCKufGzWvGoDNn8KW7Z-mOzO/lJC4BLMLWXcRZMs0yjTceRGO0RK9R9vucl1XsNSoOQI\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\n\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/BkLlGmrRyvPPVe2K9CLs5g/Xa5z1OLqQajIc3s5rUniFlqdJtaRLITP1gDihXUjv9mU-oL-UYW2vch6-LycxAE6/-WytnHsMpCFISk34tpKtqbCgebJHwBskHCmfCQuGyy4\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/a1Wo2jwD7Nm02ZkgTecnOQ/YP7vpHl89YZhPK6Auqq7XXdArGgW7P7MzRPK8phuAFxFZ1eA3krZtGvtjgJubKJw/RQDizxuE0ANsusjug0XY5b4bry7zere7SX-kWGadsBE\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/VZnCwT403jYYR4RMHBbjkw/FDfTVLEaSRANCkhdcClNBr6hvdcNvBHO8kdqvnipJC7cM6fsAAvG2lk8xCyS2u6p/e--H2BNALoRyyz8YG0G3x0XgU-G7znCqqMrLbBy9HTk\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/1Ligk6DdfkUcIPeYIK7jXw/gluQi7ZEuNJWp8FL0b1vcsmFGiwZ1V8PeyDjTgya4EtbPGtT5lCZI4OzPunRKOh7/wHHIp3maQqjoAx4Vf-KZ-42BD1k6PGxgM_odYe-E13s\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/yyxzI3AUu_1wrHeDrg01Mw/3vhh48bD9WrbGAy6dPEkLigmlptj4u36J06Tny2NpvGo8hUKjuLWuAFf_zZ1TXHXlVX9Zunx8W2Jpe-An581BQ/MtAshO4rp54zZ8mEc5fSznfz8rXALER1HhxkL693H2k\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/exEDuGNHaN1r7daUPTZZTg/S7T_N-PD9cdjLkNHccfFECVPGYr3CBueReBFWqml5IGeUECmjd3VNHEvNSJ6tuw1izsYWARx1PZOhVTBtHkj2Q/C-qkMHY4IK9BRH2A6Xs4Zsh8_iQ_ZXYF0ZNDL5rGlAQ\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/CtI9oKTks-lAyxASD4JvRw/NB-UqZlhnCUgLXlSJGO56SGkj5gMoSicF6KimvzLlZwu_3fbPbVMEQz-IM7u2GJqcrQiTb9cR4vgj2SBsiWLlg/ErWZMqQkeGOKA9KJVdgY9nm-BDC5iJZD8_lKsXtGzmw\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/ModwjU12JeR_gN8z4Tv7OA/2ITtCjxSyXdeVSo9Q2rUwR45vW69jCCqgt1pQChHqh7NQn8gZidlZSK65ZGR15ik3rX0zioErxL9Xmp2M67Vvg/cuuaPn3rJ71NahEKXqbTPWKdx6XYFyAafkCz8mHmFgE\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/nz6cidGit13btuinqJkSRA/1LNtH-aBONGMEZlPPc1UG1AmZ32VUbnkbQYnHMdv3xx6ZVJ7Y5Xv9Eu4MKO-0k2R/ow2_L8qrL-eSyBjKB9odvpgvML0-HIBmApTJOS4euEk\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/RO3CXNaWJUokuqE7JtzAcw/IGmsxrXEKduthbccTGUdxbgjnS5rklJnFqKMYL_KRgpxQWUryUbZQgR7eXTgpDcd80VUTNduVA2coqDwdaJotQ/XcdwxfaPEXTLdPgwaAtlIsGt6gDNOiweU4jti4Ow5pU\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/MHBkPj3Ck44z9y-QDIh_DQ/ApXNgwfXAkstuUME6XF2-Q50FePzBHzXIrjX1wtwlbl6ilJPNn1JXvqFDRqZkWmx6JPmbaXoQ9tNP3emNW-lJg/RIM_EsOf48girgiSQkpMhsZ8CMtQRkRMsqYz2ZfqzKs\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/umTYJ4zt-r_QonDyPYuk-w/AI4noq1xrUezZN7Q33iKwNmOTGSG0QbsnKJPx7JOGpFg284NdNWFd80AcCSmKv8xp1EuM5y-hOtPkn7lcV6n-w/ja5ALfq196NHR8SS1hLAFCyouBbc7JfdJcQaGhJZ2r0\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/4h4YOhia5UoxRQyXWlqZwQ/jAS34TE0c05wY5ApMzikxRgKfs9d_5Yt-oiK362RAmGxd4C6hJBqY5lSLIqB_nRd/Gl4fevqda9gebf0lgRBq1f9FNliocdEvNaUjO3_1QOU\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/o-A_Tt41Y72l6olys3Xaxw/ibXswx34OuWkgknAWYSjUug9IPUry6ccekRGay4vV8sdeUcqzEhL6E0dOphJu2SU/ncUWhe1gdtvyHzKZ2u5xFIq_s4AYgYNma3hwOfe7-u4\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/_ysAgK7ufcuNa0963BleZQ/xHXUuhXJ1cgIjdDDyDUVqzj7bDSeURX8xesIrtTgToq-Nm4VPpphVTcKx1xui2T5/6shOi4JaK6vANbyM0AEzqggpb5DI86zxm95JK8oOCQE\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/I8sLQV0paxtyeQv3XVbsyw/z4FgKn4yzmrNMDUgHA_jdx2RSmsatqrD4MbC99SRzcGQVKvnqNm3k6aA1bFq3jzo/PoAPjoM3h8Y70KO-A-oU1arAQ3cjgrME5p3Y8N_JubI\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/msU9gIldhQwxOtpYHWGFXw/5KY7IuIARWJHPTfCEtL_ua_1eKW5KpWusBocMIMlHjJyv1tIlNjHaSPxeljA82FNztToRReJN_f9eoc0Cdd-Jg/4cui0B_MkRHAmMc0tZapyWgEmideiIEBjAOxmjzpO70\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/bJ2ahcTM7zRlFQyh88dmqg/5NGwV-xXWws7P7nE8aZG3EGZEG3guxGShMdmfIaQpHaviMkMTS8YHOIJGLQfG4HjrMrrC7f1eVTntnUYkwSpMw/kyWPT4E1zvitVPP_ZwWkNco4YOulpsEtyRXMlJszBQg\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/lLea71o6SY3tiXC7fjZKNA/CuK5-qrLxQRxJDOUG3v5xyPVD-4ptIg0c12NK7P6LwBXpYBVV2_rC1TIIfExPpnYjXeDADQP4AdXYlbSM9cLZQ/LruLDLQ8JaulX54Wr09lQ0j5UXWHifqmDFC8pTtBpOM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/WB243jFGguabJkdrwG7s3g/BvcC3XBllFqvPBChPet3eBFFHjDU1FeGJIz3wwVkhDY3iF-pSPwmkiUQgDHM5KqE6tGgfkihzGBmGjrjK4Iq-A/hO7JlLOzJkQXXk9z10XDQbZJlfelbh_wFp4emTvSwYo\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/MrKdwR47aaSYD8brzMea8g/XpVUKBxsM8aTd1cMoSg5LYI2zcEGD9ZtsQ4Cywj0kZKFGSeeTBFDKJ4Qxd7PbUiaU-L58y5aFxqd_Hx3lZep0owMVy5Db5t5vtAedK4heP9Z74rsoX2ACGq41oLFmHfOWdWVUv3t6yFNA7rSH5X3Rw/G_UuUaXXzkDgdgpwM-ZI4wS-joN3Uxo3ct-IIyA3XfI\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/0fK-1JNu0eYGU_snrruq7Q/EIlMflTpOOFmRnW6xJCpSpjP5eRvmviufSNfzPb5nd4HUVuUV0cpX_erSf48uEolHSCOrjw9F-n5oinIrA_lfojYxa0BZ5dQQ8geOiJMUdo39QPD5-AJgt_YGQdnkaIHvtrPbKxyxNV-CFCjqXMAPg/03Up5SfYgmuL6S2YbfOAZ9KQjKPr9JDBz1gKxxapi7I\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/ICEONJBsmSo0KEZbwylxVA/hO85mp6Ll75ftNNsq7JMilKpglncY5KRPed1trzsEpeLPBHnCqNn0F3q9jV0VIfSX22PXh4Cbf6r3Ea5OWVwfOGeAaRAXQwXj-W3X3FYiy3waHLw4pJE1bmuvaEhcUK4Afb6Rf9KsILJdftUiQkApg/LVW1diDZmSRtJN9m7DHtGyfgtWWhuzFx6FYFjcOhGRc\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/kpyltB_xnbey8m1FZZm24A/-khoI9TaP-WZHU3lXz7ngVclAHYkLXoCZIByuzbHYccwAhOQYMiS-RsWJbSqoJq9PKKFyHlLY9ccXkE7Ue9KL1_SEjr0KK4ByNhL0Zw9G7BXtIKV-lj6qAc3txlsRyXXuPcV4-jrwaEwmvX1Ng_SYw/LOikCv3SBiUsMOva4oZp3trgxTa5bmlALerS6-Uh7A8\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/nX_QE9_YMUsAccWFk3qV1A/BsJoGMlkOvX8_lgPUQJdkXmNxeYW5VdgafXCprWsRDCpNprm9k5LwVSF4AWCObfYB_b_SfZBsauTgsdQ6O7FDHQvhG8xrPJxiySXwnZKm2jAKJ-V7cSTjM6WWvTKrx-ia6tAcY3OsTBhkQ1t3jcyruipSHXabGSJRVjlXw2tna6dBbPyKcqRIWxnzP81qdnE/EuMlFaw915U2Puvzt8jLmY1ie2xobrOLR5__3d-CNvY\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/oVx8KxYNg4Id3EgWG4p-aA/blsKgnqlAE0kRW4Qk5agEVSSfTI1bBZNi6Gj2DbBGPTRl0gzk9MtXJOPtVaiwScgyU-WF7JK27NbruRonbIJY6FJ5OipV_PuGcs4oooa1O3J8yFgJMOzL3oJM79FrceWhaT9xOBdgs_8lQ-u7UZnlxralIntk_Gyl6n87sWWhe3LBTUcQZH9_jXXMkHFAmUq/JLgALSozOKU7c5zJLMzW040nrnZbdG4OVzh3Q_lrXb0\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/_Y4QBEcai4IggqUflVBccg/NHYiW7KoT7ZJNej-NvJsQXivBG-k5lWL9bsyHLIrKFAZXIoCoA5PgCzPWTvatoEv9ll7JiAVHHQhZ6n4U5PouRR38CjC89HjXDOONbGFi_9GSYk0A6YWeFmzwoUlN2azrVt8dMSxnEO3vrcUc9i7I_Yc_-M1UVQVtAxZoiFvU9n58mp4nwNg6N4pNNlOqChl/8BJcohpnQjjHju4xUAPSR1e3N-BfD69ALhsABAjnyhY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/PUVNBYXo_MERyJjjAEXPew/c8p2uVHmz6sva_AQ4CynqTGfSa6Cq1XzKFDYsyLK5qQrKeOQzQHHP0GlBJLZRiketuGQJH0E6XGyl4wjovWHOTn-Mv1x28haZfIvOpdKaKGZk7Q0zkD4bq0P9KzNb1ljB45mF2KCyCG1Vry-FazbRilyou0skwDjo6omN2x4v6M9CpsZgTvT6ur1pzc9UFUv/y6JCD0Xs3QzRMbzxoXPmXKG8VFj_fsXnGUfp1icGQ3o\",\"width\":600,\"height\":600}}}]}}]}");

            bodyText = "{\"maxRecords\":3,\"timeZone\":\"Asia/Ho_Chi_Minh\",\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            task = ListAllRecords(timeZone: "Asia/Ho_Chi_Minh", maxRecords: 3);
            response = await task;
            Assert.IsTrue(response.Success);

            ////////////////////////////////////////////////////
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/fm_0j2jY_iTid2E6-jAzVA/GOpsTP1Frz94-Gay_j_q4baGhoP0D4BpRRZWXj9NZ6aI6m7JqTrRVY59aJywJlin/1rIcqXRbRdsfZV3nORMlaHtysvku-LwK8wWIxWnhwR0\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/t0KrY_xwcUvc0tkefrG7ag/ys-SjEPs3OKgNB3idXnozovknMYqIuypxRmJ6layGnQ7MWBp7H4gScQvBdlMpuzM-CY7GdJrAYGVVvN8L10G6w/_TA0XdFGG71EKJy_09ZxRUkC_w31OCRIq9IkDo7ri1Q\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/_XJPuyzWadooD8vBLl87xw/juQEodwELZ6c_Iyw2Gsd97r1r3k2V8HlenBvr76k01Tq8MjFu1OeGMh1eReLeK7_yY_DrBn7pQ_XDIFi7YpL8Q/vHjXfyeJs464-SQl1p1nb7480KGEL3oxSm-R4T21q8o\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/sS2MPSUGWPy7WKoubyUq2A/G0uMvG_iSmOgKK7UUSK9ie4x1su9rk5HAfSCTqjaWw_HBSGV-QcO3ey9GBr_hmtBZ1MdCWOVKzQ5Z4VEz7Z3cA/YV0ajIpL0TAYHKedB1ytwwgkUkWZj1Ecs9NGx15yuDw\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SNFJcSB4lMAVF_Ou7oTmHA/gF9Yg0uBVRSCgL67u9fdwLeoEAhlTjY6YSbcSSPYNFu49m4UXyVQknruuraxh4Wi/V3xJfscAHU7va0cIzn42n8n5bkLGsdA1bjfc5R5w4Ps\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/b6535IaZLoMvhG1h4-jnZw/Gx-Mol0OU8fIy5LSScM8M3NHSR9ggO-sXv5vS6UqE5Mt-6KGAIVx4_mwX2bjeQRJ/INt1n_bSTFeqe-1h0ziCVVw1jqd5cLW8yZVAPDcyaXU\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ZgCFw1P_QAWimJqx6j6dfg/bM4Jjy8hVLaIqCVPwtwrU2DP8-RhOg2v8LtcCWtrK-guQPrN4mwucP5xKGEeM5uf/3tj70-fGwQsBpGaSb852Cfb6vMQ29gejrT9ScnPw6Jc\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/rhbZ-c1sc9wcAx2xA2sY8w/tG8Q6Rsq1-qXtgKD5zU2hXitnHiBbqQW9mgtV7a5r-gNpYJHbYZciY8_UtZaobL2/uGDcWlqDXQ466e1IdHyZpPstMw5MOSzsjN4Dq6XNoyA\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/8gxbB3g-T8LV6mhKBjtmoQ/bA2SZF1T443kG1ZJu4UlJLDOA1IdasiczFIZ8HHIkvdCoLjJZflli0u04MPNSpMN/msw1tT-C5cUCux1Uuwk9sZREhw2qcV4MMnOm4yu58pU\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SzPkvjgwl28zX62-o3Yctg/GQptlaoq9ZhvDBSmL9o347ezDvjmVTs2-EgcPeVnxwxJZIOyLpDf7rFFf4gJt-k_/gZ7047myJGYaR5_UsKjksqibM2wyAfYEkB-Rw-Q2Fps\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/nwHrgxuQ_TmBpTAoyM1z6w/aDD75AzRRdZZXZ0NyGBmTuZpg7a2b84sW2CC_4Hx2ksfX0toIEycXQBf6pNXvoFg/HcuMERCnefFJiI4uefz-8xbTwWCv6-gezUjqgpbsX0M\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/igN3fz5lYmyihEAJJs5ErQ/5Tf-QVbtogNPD8hCyVjErX9VjTJigKDgqKEIFKUnOOG3r4X6x7j2_TCQ7JBpESeb/Gz51pCx2bfX_J486l9EPuY4V4RhS-t9VrIuXQnFosu0\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/XilxHFzE3jRC7LIvhfvfOA/xXGFQNI7aiaraOZhzuH9FuIk6jWZhfC8oF2ezdV_M-i0wivPdMnEuH5PzSW73Pcv/50dRonTZ-QB-ucl2e14wxNP0jy3ZGWQ9UE6yHLY3VNA\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/mDIWEWbwQtKu3WpsW71h8A/cdvE6Q4an6nXXM-mxokAu5qKJTyx3_32lBTS1mtUDg96KYHeTRi73ZVoP-VBw0A6/VxqRzAngHAqfkK3qqO1-jOIwKLSAZ2CcrgURCKCf7S0\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/x2vV8-Uhb4yNL6vV3h1l_Q/CJft6xmNCSIxcTx4kDv5tNGdz5uH4-1OENPelMXd3zwYe7XQAUtY89gOfNaj0e-2/vpMdHA2hJ_4yLlikvMQbwbJmdip1vSec3tb3YgJInVI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/j_mQGDjjTFe9RUg9fPU5OQ/cclmfynaBLyrBunXIfr9bjdCB0rC1RUfRxd2vOkLXKdX6KfNfayO1gkB4gAFdlgK/5PVrJHe2O44Ld0Nm5aGW2ap3nibKey08sHfEVUzRVGs\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\n\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/-i2mmxSu8IVEruHrAOKGrQ/Dsk1sxlsflnrlQCyKYeh-w8joxS1E4uHJCH1f_wtUFOk1W2VojRITUf7m4esY8bc/oS3lFn5YChFKL_blz35fXhwtioQX-tIi5jGyg5I1hcM\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/vXqrjr580N-mij2XgxXb3g/fgLIPmXui6BuW0alivewbijzwBtsKFIF_Ll-deZeVzvYYNTjCg1Ps05UahECkPz9/m1V7zRW6WC5bUxtSF5dCMhO09pPy9inhh3_RrAA8lFs\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/AJTRX0P5umAq9nL-ePLDxA/XCXEpMAtOKRKq23T2wrrXHS-k7EDB3WZLmhcQTs1cDiiFZM0YzoQhXkQSlu2SJ-P/NW4W66NszBjyAXmj2j7qagF_kfesAAiAKW_MnuUGORQ\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/prjUrcHvA_P0BOwjnQMIJg/ufI7uEtIleVzH222TmEFMx30_pVm-m2SwMYeOfO-eMpIpO3iEX8KcS-CYgSpWwSv/_T0kNgmRC48Qqn6-RoSwF06cyJsJsjlQTTYQGfT55KE\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/8Td82Bykr4aWqs0orIelsw/E9W-PTC_qQdQGw8JmxarqEN1LDjWkUlxtSzRvoJdmCUpOa3yYlYH-aBAzM5Ni8gYkx1ZOK_i8tBgfL4-Xx35zg/vx3AL5JwLsGoOE88OgEKiWkHCXdN82HgyvKNBE61COE\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/2jfvPZaXoLW13hvWNaRUXg/mggXi9arftRsvnAguxSjcBoVe7ztWY3vILcCuY2KmEDxloa_PZ7Z_IX7oD-88s8hFMMebOlAuGzoUhshZmx5vg/P0gBv61HcqtOVqAc2zC5zrEw9_oBoXIiSCzx4ID7RsY\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/pfv_fNFw_4Z7HPtL-40Y3A/qk8tYfkyx0nbQBUqjn8BRw7J2F2qks0mSD8N9TMck2KZe-jgOA68LIC8qkj2RMmNFRoCnB3I2T-6-VTKC2t6qQ/wtVGI2gtG6RHl-lUM0lg3UD9Y4Y81FyS8NxEssDg8Ck\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/53vTsXEmhGEck9mL6LHijQ/p_0RMKA3BZbxWtZebOHQu8uGYcwP9Y6pnJwVDlQ0DpThcAYvZuSFO9Z2PYXeBBJoBvRoH-_Act_rkmAK73Xacw/NTh72yWkY4z9O5dNW1cxRhNQ4iKLsTjj579ZTabeK_M\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ENuMTaSgh4V_iXJp9olfQQ/QbsEWV_5qIg5ylwmflHs7SrKpaqS_NqHkMXSdf2nrub0-6udYgLx7-MDPYiuBuQ9/9v_qOPL2JgcH4sMxgw_hPpEv5LxJzIQbnHuU89WOrEU\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/OOpWid7LPYtsz29Wbxl1ug/YZ8fY9K5evyHnbj0ZDvrImVadtEKVo10FAmwZ9kgFyL5Wjc0S1kl1YGfBkpQ7xARXYo15Uj5BF82ThODwwXusA/qGWy1jYh6LNji0efB-TmQZue3D-CK5vL2AuxGB89o9s\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SDw93wDBNcf2N6k3344fjw/cLhNQ87A5u47Pq8_mKKWklIUcYg8yp3holrJdbOQ_ls5Q7hmswhU644lgh7TKOjp_dsZUdnZmOfo9byngr92jg/tzLY9iJdW7aeYvJakGDgtuTf6y88Xyu04SdHs9qEsxY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/BkiD78PIFXarhsQfP2Y5sg/z3xD1sdW8_y54UUwcpJPPBSY1aS5qaERYeIKFgkN41HFSxkMHs8MgRCmrHy62wSRb8Jeb1GxRY4xyVuOl3_ExA/TMj6gkYHy-gBGS4f1sXjVTCa9XfdQB-_oV7Trt88jtY\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/5JmkzD_NNSnfl5A13q0wXw/x_zpO2UnU5-QAmCxa8ZfFYxlMyCiZ5C6VZMTeMv9Jv5GqZgfouVxbj6-Cdf2jhhX/Aqod5n4KE4JPWQFAYyFsfU5oZLoUWJWtwUsCIwvfDcc\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/kLzZUgx4XV6xQXzNxvjSwA/_XSJf5_DZP4BYZay07GrZB8awkr5jaoF3njqu487l9bG34AF6dmXWkfBUG-KS6DP/sAVvbujEfsDaKN3v1kkl0iGznujO3ox6hpVWROGkCfk\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/9c5sqk2db4feeTJXeBU6xQ/m6M3OcYTTBlEz9jTC9B2dop45g5ytE6WFdK7jdkr962RxJgCdgoJJXu00nKM9HeM/nixFapxLVQdKCkHVb1SCxN08V5qO-6Zc7ktzMZ5ulMs\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/wZsdsGNkqIkZviYYSruhAA/jRg8G1I9vFnf2iiZNj4lW1YLqGLnxYo_oN5bR2S6-mHEtv6nXUv7n_3xD0hQZeTX/gCzMUfrXPCHBjaSwacOinpN8MGf5-PbYDiEmjR3Ils4\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/hlHoHFCeo_wSW2iRbXi-IQ/5d7hAV-Di4tNU7kP6Lcw1kyOhmQmZOdTN-SSOxQtM9G8zCXHVmcqzEyZeqOqioIUArUDD4SVmUU1VA-NKeO9Xw/sx48WVHMkTGVhtBLl0QQY9S9VrShdjH7jjU-JNGuJYE\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/DSwtIDuygSFqmlRCyxKGVg/HjArKpZtpVOv6TrEoRrsCjS8XLt014LAmzJUK9g907yCJ1Xt5xY73dVp6b2BBgAIZ6LN8ezie5U4QMTUVoeypg/7kUUklUI0oxmh2a-LJszcGAlQMbA7uwDnlEPUtfZztg\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/53ZqBYezJl8iuz7BlDKhIA/wzRHFFG9PjSZEpqwFBx9fMmSEXjjIEVMCS0ZehF7quY7qZpjdIHPxZ9V0sZlApmw7LdfuO6sQmesEHmnkQc0Yg/uvCV1M1yxEVFMupBF4zV_pCsvPJV5aLN-5Ru-_kRlAk\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/RZmGCe5Y-VMoivgC_EzCXA/YxCECHWNfOthgqQGSS9TYPt36nVOhfmrEDZIIyLUYs8KXtN66bdUPQWdHJ__Skx7fdI4dBTTMDbFiPpZpSwUpg/V-ydD-u9nKOw6hYODsTnSacWAFKhwV7WAIZs3izEJDA\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/21kSQIHhcN6FxPIr2Uu4Pg/eXGLHZU9K_CExkr3LTzOWl3velWjFgmgv3oZ3rprTVw3jdXC1ez4CgwaUXHFI_FgPZm6enROUM5uTSoeqa6evuq1Zafb_SaWAIidGXjIWjIf2V0n3tg_xZWDyOaPBXfD0lnwnUETi2Il05OeBXDp3w/sGYIV2WYX2jam_scL2ZRNZj_w5CVtLeqni7ONdXjO6Y\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ctWXP7HP7yDrFfuyi5Sd5Q/9uvTiKa2Hve8EOZxBDUtfrbaeikr50d8CvTINb3mhni8fDytXCC9Ap5XkfsZMcKoHZm6cb4-9ani0cIAWleHP3s0-mO55MAa6Xks-iw4EU2IDssg7mdaDmOhB7u7jmNY877XnKSPGa2-MU6gtM08CQ/dLkikMtBVoNCQwKyMpMBhGAD_QQL9KxOCUgpO5LWxME\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/15KXpXTBtYLOWtS_1Qmgbw/5jNGpI2pQbUX4uzkpOEAJ4d2WJt09ZAH5BSzre7KtVYcyRsYKLXei4Likh1n9uOvkXOSNkiNyhSbqEjzFN_rN6B8Q5snGl1-m7D34F8ZzcgEBuMFD8jp2NWsuzFpq9vg9BLureqvPbImz5-lbzaNkQ/HvlNmhQdo91s89hBiQ3Xh22QgZxpvsTEbwhTjJMMdxk\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/rfFCTABCo2VmcJa_JEjw4g/cxvWI7gzJv7-JI_5D4ksNHkr1J7qBnoqss0QfaC8NVplU5rM0M2tOf8jQNDDpGYcCCzFlQNdw5BWzyVxTKoMQ5-yI3TcE3Z7TD5W7qZ68rlpckE5NbAWqpvFVF6UOjjoOdTKfS1HQ3XHtlYqOcTd7A/N56aCaOHioqx5ws_6pP19NwEHAtS8JxErKsobvgBXic\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/pWUuDoFUoqY12pr_VYi3aQ/Ys-O3wRiJ1DuJBZKkgqYeOy7mCAmGIuNyliCpvDsvitc8nPKM4R_gQFnx2AuShFu5NU4q7DrjzCXThXFAo0BuT4wcq653Zlo-gtD13-px41wRgJT-FCa_EAhc03fgoYSb1-h-hJWwMIGSTvbm0AcWVVlNgDzDo36qxouNQql0_LhVNT20KKL3X7gULD_nYeo/gxER7gVNJd2T31KEYOOOljBUeT5yx_lxQfhJzcMnt3o\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ZsFvvHAEfrPyMbFcXkvOSQ/nEFB5hc8ovIvd9T4vHTZUvPg2AhOZ81QKhf9rgjef4YboaPi5r4lOLI_L8F3dDLl4nO7ifdyLvLM3gHC3YbRdn1KTUZR85VPJzmeztRMdRmZkJpTNguaRdwlKwBROnZyS547LSuuPCbDipEMTiAYbAyv_QoVVK1T9kj4JYO1b9gaZnpjmm69xDO3cxFjpLh7/mAu158snDBL2d6OYITCZ-uc0tdXd7KWHPFs4t4ZaU10\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/jgl9s9wtk8_G4OPDiaATyw/2CvL883ZmUGLJqbmMO-bFhELOFHrgOugYTa-5vJbbNQ3GkJF6wx54uPCEtCq4LMlGrJBg0EkWJX4dnwdLVGgiT-T4u8IU84pKMYfWGcOMXWW8diQ2dU1U7ZWfj_rUhd9eKYSzUcTlwtEQ_wploehyyZhyhvAqwV0rD2L5NINJZUL4hfNTwB4iCOuRqu4yscW/A8adwa4wC_3ZH8M4WtKIdTcFCyStVq6q-hTgYdogdlA\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/gkqiADeE0NalIEWMhY66aw/pNc9BH-ueGj2dEn4KlemGEP_uQtmaRtIhIWyCkubXAs5-NCiZcm0YAOQuY9rJ3jGQSCN_VDQ5AXDlclSnhFL2S_i3J4cQqzMudL5LPvUqJ5Xq1N8EhwQp49D7GFadn_4YUv2dYhOL-dUAPKfQKmxU7vf9nN-_b0tZagCp8ISUcXUrRBmJHpASvb0M2EhEfF2/5ME_aSTgbx-xCuOHnqK8SJBofdrTMphEoDf5g5yGaOU\",\"width\":600,\"height\":600}}}]}}]}");

            bodyText = "{\"maxRecords\":3,\"userLocale\":\"fr-ca\",\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            task = ListAllRecords(userLocale: "fr-ca", maxRecords: 3);
            response = await task;
            Assert.IsTrue(response.Success);

            ////////////////////////////////////////////////////
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/fm_0j2jY_iTid2E6-jAzVA/GOpsTP1Frz94-Gay_j_q4baGhoP0D4BpRRZWXj9NZ6aI6m7JqTrRVY59aJywJlin/1rIcqXRbRdsfZV3nORMlaHtysvku-LwK8wWIxWnhwR0\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/t0KrY_xwcUvc0tkefrG7ag/ys-SjEPs3OKgNB3idXnozovknMYqIuypxRmJ6layGnQ7MWBp7H4gScQvBdlMpuzM-CY7GdJrAYGVVvN8L10G6w/_TA0XdFGG71EKJy_09ZxRUkC_w31OCRIq9IkDo7ri1Q\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/_XJPuyzWadooD8vBLl87xw/juQEodwELZ6c_Iyw2Gsd97r1r3k2V8HlenBvr76k01Tq8MjFu1OeGMh1eReLeK7_yY_DrBn7pQ_XDIFi7YpL8Q/vHjXfyeJs464-SQl1p1nb7480KGEL3oxSm-R4T21q8o\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/sS2MPSUGWPy7WKoubyUq2A/G0uMvG_iSmOgKK7UUSK9ie4x1su9rk5HAfSCTqjaWw_HBSGV-QcO3ey9GBr_hmtBZ1MdCWOVKzQ5Z4VEz7Z3cA/YV0ajIpL0TAYHKedB1ytwwgkUkWZj1Ecs9NGx15yuDw\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SNFJcSB4lMAVF_Ou7oTmHA/gF9Yg0uBVRSCgL67u9fdwLeoEAhlTjY6YSbcSSPYNFu49m4UXyVQknruuraxh4Wi/V3xJfscAHU7va0cIzn42n8n5bkLGsdA1bjfc5R5w4Ps\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/b6535IaZLoMvhG1h4-jnZw/Gx-Mol0OU8fIy5LSScM8M3NHSR9ggO-sXv5vS6UqE5Mt-6KGAIVx4_mwX2bjeQRJ/INt1n_bSTFeqe-1h0ziCVVw1jqd5cLW8yZVAPDcyaXU\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ZgCFw1P_QAWimJqx6j6dfg/bM4Jjy8hVLaIqCVPwtwrU2DP8-RhOg2v8LtcCWtrK-guQPrN4mwucP5xKGEeM5uf/3tj70-fGwQsBpGaSb852Cfb6vMQ29gejrT9ScnPw6Jc\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/rhbZ-c1sc9wcAx2xA2sY8w/tG8Q6Rsq1-qXtgKD5zU2hXitnHiBbqQW9mgtV7a5r-gNpYJHbYZciY8_UtZaobL2/uGDcWlqDXQ466e1IdHyZpPstMw5MOSzsjN4Dq6XNoyA\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/8gxbB3g-T8LV6mhKBjtmoQ/bA2SZF1T443kG1ZJu4UlJLDOA1IdasiczFIZ8HHIkvdCoLjJZflli0u04MPNSpMN/msw1tT-C5cUCux1Uuwk9sZREhw2qcV4MMnOm4yu58pU\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SzPkvjgwl28zX62-o3Yctg/GQptlaoq9ZhvDBSmL9o347ezDvjmVTs2-EgcPeVnxwxJZIOyLpDf7rFFf4gJt-k_/gZ7047myJGYaR5_UsKjksqibM2wyAfYEkB-Rw-Q2Fps\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/nwHrgxuQ_TmBpTAoyM1z6w/aDD75AzRRdZZXZ0NyGBmTuZpg7a2b84sW2CC_4Hx2ksfX0toIEycXQBf6pNXvoFg/HcuMERCnefFJiI4uefz-8xbTwWCv6-gezUjqgpbsX0M\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/igN3fz5lYmyihEAJJs5ErQ/5Tf-QVbtogNPD8hCyVjErX9VjTJigKDgqKEIFKUnOOG3r4X6x7j2_TCQ7JBpESeb/Gz51pCx2bfX_J486l9EPuY4V4RhS-t9VrIuXQnFosu0\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/XilxHFzE3jRC7LIvhfvfOA/xXGFQNI7aiaraOZhzuH9FuIk6jWZhfC8oF2ezdV_M-i0wivPdMnEuH5PzSW73Pcv/50dRonTZ-QB-ucl2e14wxNP0jy3ZGWQ9UE6yHLY3VNA\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/mDIWEWbwQtKu3WpsW71h8A/cdvE6Q4an6nXXM-mxokAu5qKJTyx3_32lBTS1mtUDg96KYHeTRi73ZVoP-VBw0A6/VxqRzAngHAqfkK3qqO1-jOIwKLSAZ2CcrgURCKCf7S0\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/x2vV8-Uhb4yNL6vV3h1l_Q/CJft6xmNCSIxcTx4kDv5tNGdz5uH4-1OENPelMXd3zwYe7XQAUtY89gOfNaj0e-2/vpMdHA2hJ_4yLlikvMQbwbJmdip1vSec3tb3YgJInVI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/j_mQGDjjTFe9RUg9fPU5OQ/cclmfynaBLyrBunXIfr9bjdCB0rC1RUfRxd2vOkLXKdX6KfNfayO1gkB4gAFdlgK/5PVrJHe2O44Ld0Nm5aGW2ap3nibKey08sHfEVUzRVGs\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\n\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/-i2mmxSu8IVEruHrAOKGrQ/Dsk1sxlsflnrlQCyKYeh-w8joxS1E4uHJCH1f_wtUFOk1W2VojRITUf7m4esY8bc/oS3lFn5YChFKL_blz35fXhwtioQX-tIi5jGyg5I1hcM\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/vXqrjr580N-mij2XgxXb3g/fgLIPmXui6BuW0alivewbijzwBtsKFIF_Ll-deZeVzvYYNTjCg1Ps05UahECkPz9/m1V7zRW6WC5bUxtSF5dCMhO09pPy9inhh3_RrAA8lFs\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/AJTRX0P5umAq9nL-ePLDxA/XCXEpMAtOKRKq23T2wrrXHS-k7EDB3WZLmhcQTs1cDiiFZM0YzoQhXkQSlu2SJ-P/NW4W66NszBjyAXmj2j7qagF_kfesAAiAKW_MnuUGORQ\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/prjUrcHvA_P0BOwjnQMIJg/ufI7uEtIleVzH222TmEFMx30_pVm-m2SwMYeOfO-eMpIpO3iEX8KcS-CYgSpWwSv/_T0kNgmRC48Qqn6-RoSwF06cyJsJsjlQTTYQGfT55KE\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/8Td82Bykr4aWqs0orIelsw/E9W-PTC_qQdQGw8JmxarqEN1LDjWkUlxtSzRvoJdmCUpOa3yYlYH-aBAzM5Ni8gYkx1ZOK_i8tBgfL4-Xx35zg/vx3AL5JwLsGoOE88OgEKiWkHCXdN82HgyvKNBE61COE\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/2jfvPZaXoLW13hvWNaRUXg/mggXi9arftRsvnAguxSjcBoVe7ztWY3vILcCuY2KmEDxloa_PZ7Z_IX7oD-88s8hFMMebOlAuGzoUhshZmx5vg/P0gBv61HcqtOVqAc2zC5zrEw9_oBoXIiSCzx4ID7RsY\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/pfv_fNFw_4Z7HPtL-40Y3A/qk8tYfkyx0nbQBUqjn8BRw7J2F2qks0mSD8N9TMck2KZe-jgOA68LIC8qkj2RMmNFRoCnB3I2T-6-VTKC2t6qQ/wtVGI2gtG6RHl-lUM0lg3UD9Y4Y81FyS8NxEssDg8Ck\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/53vTsXEmhGEck9mL6LHijQ/p_0RMKA3BZbxWtZebOHQu8uGYcwP9Y6pnJwVDlQ0DpThcAYvZuSFO9Z2PYXeBBJoBvRoH-_Act_rkmAK73Xacw/NTh72yWkY4z9O5dNW1cxRhNQ4iKLsTjj579ZTabeK_M\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ENuMTaSgh4V_iXJp9olfQQ/QbsEWV_5qIg5ylwmflHs7SrKpaqS_NqHkMXSdf2nrub0-6udYgLx7-MDPYiuBuQ9/9v_qOPL2JgcH4sMxgw_hPpEv5LxJzIQbnHuU89WOrEU\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/OOpWid7LPYtsz29Wbxl1ug/YZ8fY9K5evyHnbj0ZDvrImVadtEKVo10FAmwZ9kgFyL5Wjc0S1kl1YGfBkpQ7xARXYo15Uj5BF82ThODwwXusA/qGWy1jYh6LNji0efB-TmQZue3D-CK5vL2AuxGB89o9s\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SDw93wDBNcf2N6k3344fjw/cLhNQ87A5u47Pq8_mKKWklIUcYg8yp3holrJdbOQ_ls5Q7hmswhU644lgh7TKOjp_dsZUdnZmOfo9byngr92jg/tzLY9iJdW7aeYvJakGDgtuTf6y88Xyu04SdHs9qEsxY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/BkiD78PIFXarhsQfP2Y5sg/z3xD1sdW8_y54UUwcpJPPBSY1aS5qaERYeIKFgkN41HFSxkMHs8MgRCmrHy62wSRb8Jeb1GxRY4xyVuOl3_ExA/TMj6gkYHy-gBGS4f1sXjVTCa9XfdQB-_oV7Trt88jtY\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/5JmkzD_NNSnfl5A13q0wXw/x_zpO2UnU5-QAmCxa8ZfFYxlMyCiZ5C6VZMTeMv9Jv5GqZgfouVxbj6-Cdf2jhhX/Aqod5n4KE4JPWQFAYyFsfU5oZLoUWJWtwUsCIwvfDcc\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/kLzZUgx4XV6xQXzNxvjSwA/_XSJf5_DZP4BYZay07GrZB8awkr5jaoF3njqu487l9bG34AF6dmXWkfBUG-KS6DP/sAVvbujEfsDaKN3v1kkl0iGznujO3ox6hpVWROGkCfk\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/9c5sqk2db4feeTJXeBU6xQ/m6M3OcYTTBlEz9jTC9B2dop45g5ytE6WFdK7jdkr962RxJgCdgoJJXu00nKM9HeM/nixFapxLVQdKCkHVb1SCxN08V5qO-6Zc7ktzMZ5ulMs\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/wZsdsGNkqIkZviYYSruhAA/jRg8G1I9vFnf2iiZNj4lW1YLqGLnxYo_oN5bR2S6-mHEtv6nXUv7n_3xD0hQZeTX/gCzMUfrXPCHBjaSwacOinpN8MGf5-PbYDiEmjR3Ils4\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/hlHoHFCeo_wSW2iRbXi-IQ/5d7hAV-Di4tNU7kP6Lcw1kyOhmQmZOdTN-SSOxQtM9G8zCXHVmcqzEyZeqOqioIUArUDD4SVmUU1VA-NKeO9Xw/sx48WVHMkTGVhtBLl0QQY9S9VrShdjH7jjU-JNGuJYE\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/DSwtIDuygSFqmlRCyxKGVg/HjArKpZtpVOv6TrEoRrsCjS8XLt014LAmzJUK9g907yCJ1Xt5xY73dVp6b2BBgAIZ6LN8ezie5U4QMTUVoeypg/7kUUklUI0oxmh2a-LJszcGAlQMbA7uwDnlEPUtfZztg\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/53ZqBYezJl8iuz7BlDKhIA/wzRHFFG9PjSZEpqwFBx9fMmSEXjjIEVMCS0ZehF7quY7qZpjdIHPxZ9V0sZlApmw7LdfuO6sQmesEHmnkQc0Yg/uvCV1M1yxEVFMupBF4zV_pCsvPJV5aLN-5Ru-_kRlAk\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/RZmGCe5Y-VMoivgC_EzCXA/YxCECHWNfOthgqQGSS9TYPt36nVOhfmrEDZIIyLUYs8KXtN66bdUPQWdHJ__Skx7fdI4dBTTMDbFiPpZpSwUpg/V-ydD-u9nKOw6hYODsTnSacWAFKhwV7WAIZs3izEJDA\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/21kSQIHhcN6FxPIr2Uu4Pg/eXGLHZU9K_CExkr3LTzOWl3velWjFgmgv3oZ3rprTVw3jdXC1ez4CgwaUXHFI_FgPZm6enROUM5uTSoeqa6evuq1Zafb_SaWAIidGXjIWjIf2V0n3tg_xZWDyOaPBXfD0lnwnUETi2Il05OeBXDp3w/sGYIV2WYX2jam_scL2ZRNZj_w5CVtLeqni7ONdXjO6Y\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ctWXP7HP7yDrFfuyi5Sd5Q/9uvTiKa2Hve8EOZxBDUtfrbaeikr50d8CvTINb3mhni8fDytXCC9Ap5XkfsZMcKoHZm6cb4-9ani0cIAWleHP3s0-mO55MAa6Xks-iw4EU2IDssg7mdaDmOhB7u7jmNY877XnKSPGa2-MU6gtM08CQ/dLkikMtBVoNCQwKyMpMBhGAD_QQL9KxOCUgpO5LWxME\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/15KXpXTBtYLOWtS_1Qmgbw/5jNGpI2pQbUX4uzkpOEAJ4d2WJt09ZAH5BSzre7KtVYcyRsYKLXei4Likh1n9uOvkXOSNkiNyhSbqEjzFN_rN6B8Q5snGl1-m7D34F8ZzcgEBuMFD8jp2NWsuzFpq9vg9BLureqvPbImz5-lbzaNkQ/HvlNmhQdo91s89hBiQ3Xh22QgZxpvsTEbwhTjJMMdxk\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/rfFCTABCo2VmcJa_JEjw4g/cxvWI7gzJv7-JI_5D4ksNHkr1J7qBnoqss0QfaC8NVplU5rM0M2tOf8jQNDDpGYcCCzFlQNdw5BWzyVxTKoMQ5-yI3TcE3Z7TD5W7qZ68rlpckE5NbAWqpvFVF6UOjjoOdTKfS1HQ3XHtlYqOcTd7A/N56aCaOHioqx5ws_6pP19NwEHAtS8JxErKsobvgBXic\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/pWUuDoFUoqY12pr_VYi3aQ/Ys-O3wRiJ1DuJBZKkgqYeOy7mCAmGIuNyliCpvDsvitc8nPKM4R_gQFnx2AuShFu5NU4q7DrjzCXThXFAo0BuT4wcq653Zlo-gtD13-px41wRgJT-FCa_EAhc03fgoYSb1-h-hJWwMIGSTvbm0AcWVVlNgDzDo36qxouNQql0_LhVNT20KKL3X7gULD_nYeo/gxER7gVNJd2T31KEYOOOljBUeT5yx_lxQfhJzcMnt3o\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ZsFvvHAEfrPyMbFcXkvOSQ/nEFB5hc8ovIvd9T4vHTZUvPg2AhOZ81QKhf9rgjef4YboaPi5r4lOLI_L8F3dDLl4nO7ifdyLvLM3gHC3YbRdn1KTUZR85VPJzmeztRMdRmZkJpTNguaRdwlKwBROnZyS547LSuuPCbDipEMTiAYbAyv_QoVVK1T9kj4JYO1b9gaZnpjmm69xDO3cxFjpLh7/mAu158snDBL2d6OYITCZ-uc0tdXd7KWHPFs4t4ZaU10\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/jgl9s9wtk8_G4OPDiaATyw/2CvL883ZmUGLJqbmMO-bFhELOFHrgOugYTa-5vJbbNQ3GkJF6wx54uPCEtCq4LMlGrJBg0EkWJX4dnwdLVGgiT-T4u8IU84pKMYfWGcOMXWW8diQ2dU1U7ZWfj_rUhd9eKYSzUcTlwtEQ_wploehyyZhyhvAqwV0rD2L5NINJZUL4hfNTwB4iCOuRqu4yscW/A8adwa4wC_3ZH8M4WtKIdTcFCyStVq6q-hTgYdogdlA\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/gkqiADeE0NalIEWMhY66aw/pNc9BH-ueGj2dEn4KlemGEP_uQtmaRtIhIWyCkubXAs5-NCiZcm0YAOQuY9rJ3jGQSCN_VDQ5AXDlclSnhFL2S_i3J4cQqzMudL5LPvUqJ5Xq1N8EhwQp49D7GFadn_4YUv2dYhOL-dUAPKfQKmxU7vf9nN-_b0tZagCp8ISUcXUrRBmJHpASvb0M2EhEfF2/5ME_aSTgbx-xCuOHnqK8SJBofdrTMphEoDf5g5yGaOU\",\"width\":600,\"height\":600}}}]}}]}");

            bodyText = "{\"maxRecords\":3,\"timeZone\":\"Asia/Ho_Chi_Minh\",\"userLocale\":\"fr-ca\",\"returnFieldsByFieldId\":false}";
            
            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "/listRecords",
                HttpMethod.Post,
                fakeResponse,
                bodyText);

            task = ListAllRecords(timeZone: "Asia/Ho_Chi_Minh", userLocale: "fr-ca", maxRecords: 3);
            response = await task;
            Assert.IsTrue(response.Success);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzCAtApiClientListRecordsCellFormatErrorTest
        // List records
        // Returned records do not include any fields with "empty" values, e.g. "", [], or false.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzCAtApiClientListRecordsCellFormatErrorTest()
        {
            fakeResponse.Content = new StringContent
                ("");   // dummy content

            fakeResponseHandler.AddFakeResponse(
                BASE_URL + "?cellFormat=string",
                HttpMethod.Get,
                fakeResponse);

            Task<ListAllRecordsTestResponse> task = ListAllRecords(cellFormat: "string");
            var response = await task;
            Assert.IsFalse(response.Success);

            task = ListAllRecords(cellFormat: "string", timeZone: "Asia/Ho_Chi_Minh");
            response = await task;
            Assert.IsFalse(response.Success);

            task = ListAllRecords(cellFormat: "string", userLocale: "fr-ca");
            response = await task;
            Assert.IsFalse(response.Success);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzDAtApiClientTooManyRecordsErrorTest
        // Multiple-records operation
        // Returned records do not include any fields with "empty" values, e.g. "", [], or false.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzDAtApiClientTooManyRecordsErrorTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/fm_0j2jY_iTid2E6-jAzVA/GOpsTP1Frz94-Gay_j_q4baGhoP0D4BpRRZWXj9NZ6aI6m7JqTrRVY59aJywJlin/1rIcqXRbRdsfZV3nORMlaHtysvku-LwK8wWIxWnhwR0\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/t0KrY_xwcUvc0tkefrG7ag/ys-SjEPs3OKgNB3idXnozovknMYqIuypxRmJ6layGnQ7MWBp7H4gScQvBdlMpuzM-CY7GdJrAYGVVvN8L10G6w/_TA0XdFGG71EKJy_09ZxRUkC_w31OCRIq9IkDo7ri1Q\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/_XJPuyzWadooD8vBLl87xw/juQEodwELZ6c_Iyw2Gsd97r1r3k2V8HlenBvr76k01Tq8MjFu1OeGMh1eReLeK7_yY_DrBn7pQ_XDIFi7YpL8Q/vHjXfyeJs464-SQl1p1nb7480KGEL3oxSm-R4T21q8o\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/sS2MPSUGWPy7WKoubyUq2A/G0uMvG_iSmOgKK7UUSK9ie4x1su9rk5HAfSCTqjaWw_HBSGV-QcO3ey9GBr_hmtBZ1MdCWOVKzQ5Z4VEz7Z3cA/YV0ajIpL0TAYHKedB1ytwwgkUkWZj1Ecs9NGx15yuDw\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SNFJcSB4lMAVF_Ou7oTmHA/gF9Yg0uBVRSCgL67u9fdwLeoEAhlTjY6YSbcSSPYNFu49m4UXyVQknruuraxh4Wi/V3xJfscAHU7va0cIzn42n8n5bkLGsdA1bjfc5R5w4Ps\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/b6535IaZLoMvhG1h4-jnZw/Gx-Mol0OU8fIy5LSScM8M3NHSR9ggO-sXv5vS6UqE5Mt-6KGAIVx4_mwX2bjeQRJ/INt1n_bSTFeqe-1h0ziCVVw1jqd5cLW8yZVAPDcyaXU\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ZgCFw1P_QAWimJqx6j6dfg/bM4Jjy8hVLaIqCVPwtwrU2DP8-RhOg2v8LtcCWtrK-guQPrN4mwucP5xKGEeM5uf/3tj70-fGwQsBpGaSb852Cfb6vMQ29gejrT9ScnPw6Jc\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/rhbZ-c1sc9wcAx2xA2sY8w/tG8Q6Rsq1-qXtgKD5zU2hXitnHiBbqQW9mgtV7a5r-gNpYJHbYZciY8_UtZaobL2/uGDcWlqDXQ466e1IdHyZpPstMw5MOSzsjN4Dq6XNoyA\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/8gxbB3g-T8LV6mhKBjtmoQ/bA2SZF1T443kG1ZJu4UlJLDOA1IdasiczFIZ8HHIkvdCoLjJZflli0u04MPNSpMN/msw1tT-C5cUCux1Uuwk9sZREhw2qcV4MMnOm4yu58pU\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SzPkvjgwl28zX62-o3Yctg/GQptlaoq9ZhvDBSmL9o347ezDvjmVTs2-EgcPeVnxwxJZIOyLpDf7rFFf4gJt-k_/gZ7047myJGYaR5_UsKjksqibM2wyAfYEkB-Rw-Q2Fps\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/nwHrgxuQ_TmBpTAoyM1z6w/aDD75AzRRdZZXZ0NyGBmTuZpg7a2b84sW2CC_4Hx2ksfX0toIEycXQBf6pNXvoFg/HcuMERCnefFJiI4uefz-8xbTwWCv6-gezUjqgpbsX0M\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/igN3fz5lYmyihEAJJs5ErQ/5Tf-QVbtogNPD8hCyVjErX9VjTJigKDgqKEIFKUnOOG3r4X6x7j2_TCQ7JBpESeb/Gz51pCx2bfX_J486l9EPuY4V4RhS-t9VrIuXQnFosu0\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/XilxHFzE3jRC7LIvhfvfOA/xXGFQNI7aiaraOZhzuH9FuIk6jWZhfC8oF2ezdV_M-i0wivPdMnEuH5PzSW73Pcv/50dRonTZ-QB-ucl2e14wxNP0jy3ZGWQ9UE6yHLY3VNA\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/mDIWEWbwQtKu3WpsW71h8A/cdvE6Q4an6nXXM-mxokAu5qKJTyx3_32lBTS1mtUDg96KYHeTRi73ZVoP-VBw0A6/VxqRzAngHAqfkK3qqO1-jOIwKLSAZ2CcrgURCKCf7S0\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/x2vV8-Uhb4yNL6vV3h1l_Q/CJft6xmNCSIxcTx4kDv5tNGdz5uH4-1OENPelMXd3zwYe7XQAUtY89gOfNaj0e-2/vpMdHA2hJ_4yLlikvMQbwbJmdip1vSec3tb3YgJInVI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/j_mQGDjjTFe9RUg9fPU5OQ/cclmfynaBLyrBunXIfr9bjdCB0rC1RUfRxd2vOkLXKdX6KfNfayO1gkB4gAFdlgK/5PVrJHe2O44Ld0Nm5aGW2ap3nibKey08sHfEVUzRVGs\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\n\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/-i2mmxSu8IVEruHrAOKGrQ/Dsk1sxlsflnrlQCyKYeh-w8joxS1E4uHJCH1f_wtUFOk1W2VojRITUf7m4esY8bc/oS3lFn5YChFKL_blz35fXhwtioQX-tIi5jGyg5I1hcM\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/vXqrjr580N-mij2XgxXb3g/fgLIPmXui6BuW0alivewbijzwBtsKFIF_Ll-deZeVzvYYNTjCg1Ps05UahECkPz9/m1V7zRW6WC5bUxtSF5dCMhO09pPy9inhh3_RrAA8lFs\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/AJTRX0P5umAq9nL-ePLDxA/XCXEpMAtOKRKq23T2wrrXHS-k7EDB3WZLmhcQTs1cDiiFZM0YzoQhXkQSlu2SJ-P/NW4W66NszBjyAXmj2j7qagF_kfesAAiAKW_MnuUGORQ\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/prjUrcHvA_P0BOwjnQMIJg/ufI7uEtIleVzH222TmEFMx30_pVm-m2SwMYeOfO-eMpIpO3iEX8KcS-CYgSpWwSv/_T0kNgmRC48Qqn6-RoSwF06cyJsJsjlQTTYQGfT55KE\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/8Td82Bykr4aWqs0orIelsw/E9W-PTC_qQdQGw8JmxarqEN1LDjWkUlxtSzRvoJdmCUpOa3yYlYH-aBAzM5Ni8gYkx1ZOK_i8tBgfL4-Xx35zg/vx3AL5JwLsGoOE88OgEKiWkHCXdN82HgyvKNBE61COE\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/2jfvPZaXoLW13hvWNaRUXg/mggXi9arftRsvnAguxSjcBoVe7ztWY3vILcCuY2KmEDxloa_PZ7Z_IX7oD-88s8hFMMebOlAuGzoUhshZmx5vg/P0gBv61HcqtOVqAc2zC5zrEw9_oBoXIiSCzx4ID7RsY\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/pfv_fNFw_4Z7HPtL-40Y3A/qk8tYfkyx0nbQBUqjn8BRw7J2F2qks0mSD8N9TMck2KZe-jgOA68LIC8qkj2RMmNFRoCnB3I2T-6-VTKC2t6qQ/wtVGI2gtG6RHl-lUM0lg3UD9Y4Y81FyS8NxEssDg8Ck\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/53vTsXEmhGEck9mL6LHijQ/p_0RMKA3BZbxWtZebOHQu8uGYcwP9Y6pnJwVDlQ0DpThcAYvZuSFO9Z2PYXeBBJoBvRoH-_Act_rkmAK73Xacw/NTh72yWkY4z9O5dNW1cxRhNQ4iKLsTjj579ZTabeK_M\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ENuMTaSgh4V_iXJp9olfQQ/QbsEWV_5qIg5ylwmflHs7SrKpaqS_NqHkMXSdf2nrub0-6udYgLx7-MDPYiuBuQ9/9v_qOPL2JgcH4sMxgw_hPpEv5LxJzIQbnHuU89WOrEU\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/OOpWid7LPYtsz29Wbxl1ug/YZ8fY9K5evyHnbj0ZDvrImVadtEKVo10FAmwZ9kgFyL5Wjc0S1kl1YGfBkpQ7xARXYo15Uj5BF82ThODwwXusA/qGWy1jYh6LNji0efB-TmQZue3D-CK5vL2AuxGB89o9s\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SDw93wDBNcf2N6k3344fjw/cLhNQ87A5u47Pq8_mKKWklIUcYg8yp3holrJdbOQ_ls5Q7hmswhU644lgh7TKOjp_dsZUdnZmOfo9byngr92jg/tzLY9iJdW7aeYvJakGDgtuTf6y88Xyu04SdHs9qEsxY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/BkiD78PIFXarhsQfP2Y5sg/z3xD1sdW8_y54UUwcpJPPBSY1aS5qaERYeIKFgkN41HFSxkMHs8MgRCmrHy62wSRb8Jeb1GxRY4xyVuOl3_ExA/TMj6gkYHy-gBGS4f1sXjVTCa9XfdQB-_oV7Trt88jtY\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/5JmkzD_NNSnfl5A13q0wXw/x_zpO2UnU5-QAmCxa8ZfFYxlMyCiZ5C6VZMTeMv9Jv5GqZgfouVxbj6-Cdf2jhhX/Aqod5n4KE4JPWQFAYyFsfU5oZLoUWJWtwUsCIwvfDcc\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/kLzZUgx4XV6xQXzNxvjSwA/_XSJf5_DZP4BYZay07GrZB8awkr5jaoF3njqu487l9bG34AF6dmXWkfBUG-KS6DP/sAVvbujEfsDaKN3v1kkl0iGznujO3ox6hpVWROGkCfk\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/9c5sqk2db4feeTJXeBU6xQ/m6M3OcYTTBlEz9jTC9B2dop45g5ytE6WFdK7jdkr962RxJgCdgoJJXu00nKM9HeM/nixFapxLVQdKCkHVb1SCxN08V5qO-6Zc7ktzMZ5ulMs\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/wZsdsGNkqIkZviYYSruhAA/jRg8G1I9vFnf2iiZNj4lW1YLqGLnxYo_oN5bR2S6-mHEtv6nXUv7n_3xD0hQZeTX/gCzMUfrXPCHBjaSwacOinpN8MGf5-PbYDiEmjR3Ils4\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/hlHoHFCeo_wSW2iRbXi-IQ/5d7hAV-Di4tNU7kP6Lcw1kyOhmQmZOdTN-SSOxQtM9G8zCXHVmcqzEyZeqOqioIUArUDD4SVmUU1VA-NKeO9Xw/sx48WVHMkTGVhtBLl0QQY9S9VrShdjH7jjU-JNGuJYE\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/DSwtIDuygSFqmlRCyxKGVg/HjArKpZtpVOv6TrEoRrsCjS8XLt014LAmzJUK9g907yCJ1Xt5xY73dVp6b2BBgAIZ6LN8ezie5U4QMTUVoeypg/7kUUklUI0oxmh2a-LJszcGAlQMbA7uwDnlEPUtfZztg\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/53ZqBYezJl8iuz7BlDKhIA/wzRHFFG9PjSZEpqwFBx9fMmSEXjjIEVMCS0ZehF7quY7qZpjdIHPxZ9V0sZlApmw7LdfuO6sQmesEHmnkQc0Yg/uvCV1M1yxEVFMupBF4zV_pCsvPJV5aLN-5Ru-_kRlAk\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/RZmGCe5Y-VMoivgC_EzCXA/YxCECHWNfOthgqQGSS9TYPt36nVOhfmrEDZIIyLUYs8KXtN66bdUPQWdHJ__Skx7fdI4dBTTMDbFiPpZpSwUpg/V-ydD-u9nKOw6hYODsTnSacWAFKhwV7WAIZs3izEJDA\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/21kSQIHhcN6FxPIr2Uu4Pg/eXGLHZU9K_CExkr3LTzOWl3velWjFgmgv3oZ3rprTVw3jdXC1ez4CgwaUXHFI_FgPZm6enROUM5uTSoeqa6evuq1Zafb_SaWAIidGXjIWjIf2V0n3tg_xZWDyOaPBXfD0lnwnUETi2Il05OeBXDp3w/sGYIV2WYX2jam_scL2ZRNZj_w5CVtLeqni7ONdXjO6Y\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ctWXP7HP7yDrFfuyi5Sd5Q/9uvTiKa2Hve8EOZxBDUtfrbaeikr50d8CvTINb3mhni8fDytXCC9Ap5XkfsZMcKoHZm6cb4-9ani0cIAWleHP3s0-mO55MAa6Xks-iw4EU2IDssg7mdaDmOhB7u7jmNY877XnKSPGa2-MU6gtM08CQ/dLkikMtBVoNCQwKyMpMBhGAD_QQL9KxOCUgpO5LWxME\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/15KXpXTBtYLOWtS_1Qmgbw/5jNGpI2pQbUX4uzkpOEAJ4d2WJt09ZAH5BSzre7KtVYcyRsYKLXei4Likh1n9uOvkXOSNkiNyhSbqEjzFN_rN6B8Q5snGl1-m7D34F8ZzcgEBuMFD8jp2NWsuzFpq9vg9BLureqvPbImz5-lbzaNkQ/HvlNmhQdo91s89hBiQ3Xh22QgZxpvsTEbwhTjJMMdxk\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/rfFCTABCo2VmcJa_JEjw4g/cxvWI7gzJv7-JI_5D4ksNHkr1J7qBnoqss0QfaC8NVplU5rM0M2tOf8jQNDDpGYcCCzFlQNdw5BWzyVxTKoMQ5-yI3TcE3Z7TD5W7qZ68rlpckE5NbAWqpvFVF6UOjjoOdTKfS1HQ3XHtlYqOcTd7A/N56aCaOHioqx5ws_6pP19NwEHAtS8JxErKsobvgBXic\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/pWUuDoFUoqY12pr_VYi3aQ/Ys-O3wRiJ1DuJBZKkgqYeOy7mCAmGIuNyliCpvDsvitc8nPKM4R_gQFnx2AuShFu5NU4q7DrjzCXThXFAo0BuT4wcq653Zlo-gtD13-px41wRgJT-FCa_EAhc03fgoYSb1-h-hJWwMIGSTvbm0AcWVVlNgDzDo36qxouNQql0_LhVNT20KKL3X7gULD_nYeo/gxER7gVNJd2T31KEYOOOljBUeT5yx_lxQfhJzcMnt3o\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ZsFvvHAEfrPyMbFcXkvOSQ/nEFB5hc8ovIvd9T4vHTZUvPg2AhOZ81QKhf9rgjef4YboaPi5r4lOLI_L8F3dDLl4nO7ifdyLvLM3gHC3YbRdn1KTUZR85VPJzmeztRMdRmZkJpTNguaRdwlKwBROnZyS547LSuuPCbDipEMTiAYbAyv_QoVVK1T9kj4JYO1b9gaZnpjmm69xDO3cxFjpLh7/mAu158snDBL2d6OYITCZ-uc0tdXd7KWHPFs4t4ZaU10\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/jgl9s9wtk8_G4OPDiaATyw/2CvL883ZmUGLJqbmMO-bFhELOFHrgOugYTa-5vJbbNQ3GkJF6wx54uPCEtCq4LMlGrJBg0EkWJX4dnwdLVGgiT-T4u8IU84pKMYfWGcOMXWW8diQ2dU1U7ZWfj_rUhd9eKYSzUcTlwtEQ_wploehyyZhyhvAqwV0rD2L5NINJZUL4hfNTwB4iCOuRqu4yscW/A8adwa4wC_3ZH8M4WtKIdTcFCyStVq6q-hTgYdogdlA\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/gkqiADeE0NalIEWMhY66aw/pNc9BH-ueGj2dEn4KlemGEP_uQtmaRtIhIWyCkubXAs5-NCiZcm0YAOQuY9rJ3jGQSCN_VDQ5AXDlclSnhFL2S_i3J4cQqzMudL5LPvUqJ5Xq1N8EhwQp49D7GFadn_4YUv2dYhOL-dUAPKfQKmxU7vf9nN-_b0tZagCp8ISUcXUrRBmJHpASvb0M2EhEfF2/5ME_aSTgbx-xCuOHnqK8SJBofdrTMphEoDf5g5yGaOU\",\"width\":600,\"height\":600}}}]}},{\"id\":\"recaaJrI2JbRgEX5O\",\"createdTime\":\"2015-02-10T00:15:45.000Z\",\"fields\":{\"Genre\":[\"Abstract Expressionism\",\"Modern art\",\"Surrealism\"],\"Bio\":\"Edvard Munch was a Norwegian painter and printmaker whose intensely evocative treatment of psychological themes built upon some of the main tenets of late 19th-century Symbolism and greatly influenced German Expressionism in the early 20th century. One of his most well-known works is The Scream of 1893.\\n\\n\",\"Name\":\"Edvard Munch\",\"Testing Date\":\"2012-02-01T23:36:00.000Z\",\"Collection\":[\"recwpd7MLPQqorfcj\"],\"Attachments\":[{\"id\":\"attNIEYhExe4q53lp\",\"width\":1000,\"height\":579,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/izQ3GivDSBQp7CS8g5KONA/Q4usEw0TkRx36TJ5SCGoLmsOWjGDxPVkNHSkZNOcoAJOwZbu2BBVCghnWsqKxd-s/NWpxtpQ5BZmm8QzD1WRoo9oaZUqUBD_izQwMvejgaiY\",\"filename\":\"The_Sun.jpg\",\"size\":194051,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/_lzaF06KD9Jom6wpoafKew/-TviqZZv0cmKWbCaWCTPAFWE7wKlLnaJUS7RubD1o1z2ZTe7AB87Myj8jiu2jEoQ/ohFs5pPxqP7iHzSBqkU6eD49AFG30X7QDiUbARm6sjI\",\"width\":62,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/6-qoO6InUVMdxZ8dVHTKlA/s37bUoNUcuSSlnVXSbbGz_RhkpA0crORJEe_PI1HWEqDAzYpHolbX1bnjJ_0QicS/98b464uh1Agu5VcyPK20wRJ7Nta32nfEIvDbuC8x_ks\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/Gx32N7Ug4UFipK-cpyfxRw/jNQrdb5M2_ks13MoAj_-6vraDMBx9_d5BwnvC3F-q1f3_UzsrQDcHjH_pV0jk0GX/dROI-g-T2aL-uSDRqh-uxs90tSOxjIurXva7SE0UIyY\",\"width\":1000,\"height\":579}}},{\"id\":\"attVjzN5X8xdWoc2W\",\"width\":1458,\"height\":1500,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/4rtgon9R580u0os50yo1zw/hpTwFixpkBxq7iJgCzehsrUZxDnqkev3nL1geiMjop3LWJ1cpkE_d5y_I6bWhE51eFa2LNHfDCKScTU3QfhPaA/fA0QOuwjcB_wjKjm9Uukyl1RAl5E9m87aaRi4K7MTiU\",\"filename\":\"Munch_Det_Syke_Barn_1896.jpg\",\"size\":425603,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/1cgpSV85x5H_Lg7b1PAATQ/wjAGpiwKwx7zD7Qx2H_Z2t6NM12zFO7nFhHfmUDnY8THqNAnmbhPBI7LPHvsERf9DrDRYkXbq-MnUhD7kZIwgQ/q_U3AoLu1VUr94E0Ldn6sgrztTvjBas3ohY8KFeOw_Y\",\"width\":35,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/hmXhYL9xh7ytvS0gQzoVaQ/_aepJTNgvO0oCEXLo29FauYlyJNptWIufEmih_KjxF0h75AljRn4b5Ku1sr0qul_vfKcDvobATmpIOg877Nc6Q/jnVuwGdNjYcKbWSk8opox8CPkTADww6Orp9z32MsRJM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/oKJi37y_XgwgTUhqY6e9vw/448UZM9FLIi6TAB-LcyzzuK8oFMYwpPUAQT86jRdy2o6vBXfjcMfxw6sjlc55llXMyHhFEpY_jVkfIJYdOFEmg/S9TCQZznyibA3yFC5-nZP_cNFBuhsag0H7TLfRATKD4\",\"width\":1458,\"height\":1500}}},{\"id\":\"attnTOZBfCiHQhfy1\",\"width\":850,\"height\":765,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/EbStN9nMezZy3tEiNBL1nQ/VgZnHI5i5WZURzifjeAkLUPOvviBSw_39AIA0nFvbslc8XKMAq2lT0Qkv2Amjvs3/xkCgGvD6R-XPQqBhEJoeDaqPVvVnCpwEKpU80wakdTE\",\"filename\":\"death-in-the-sickroom.jpg\",\"size\":255101,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/2Z2xHWIEj6B0aASnp0JkSA/PgMRvxsFbvqBMXybiVA_0JeRe6_4C9XnqDmg4Y9aUztl7-wX_Uv60bn0itKU0ClBo7cRVvWJchdoULi7SeTzzQ/jFs14L2uk8TUatW9mbmCf41dVNNnUyKl0qxYM6cqgcs\",\"width\":40,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/0IDQ3kxljhOZf-GiNIk-0g/_3XgXzHKT1avGb724es4ARKBAYMxtezbjDeeEXyQwtq_myVrKAM5_wDF_wxqnKE1qQBhu61ZjYvGklagPjgEAw/Vhi8nTkMvXhX8E_LBcIWQF66nR8A_wHLNHMHSf76FHA\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/_KsYmxT9ZAAfkTj1e0dIhw/5pQKllxXhLIbQKhH48XeqHkbiXKryTFb549tyKQ6Cko2jYZUCgAYiKpn3I4_qjdo3fHcob_-jOra81ok9GYoqA/6ptm0MTvhZ7WU-ScdCOfJyuryA2I9K5WaB12MvqikO0\",\"width\":850,\"height\":765}}}]}},{\"id\":\"recj31Rc5TXAiVZV3\",\"createdTime\":\"2015-02-09T23:36:53.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"Experimental Sculpture\"],\"Bio\":\"Isamu Noguchi (野口 勇) was a prominent Japanese American artist and landscape architect whose artistic career spanned six decades, from the 1920s onward. Known for his sculpture and public works, Noguchi also designed stage sets for various Martha Graham productions, and several mass-produced lamps and furniture pieces, some of which are still manufactured and sold.\",\"Name\":\"Isamu Noguchi\",\"Collection\":[\"reccV1ddwIspBOe4O\"],\"Attachments\":[{\"id\":\"attuoGtQSGoeWEurX\",\"width\":640,\"height\":487,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/wvxndPWah6jim9UaOpBnYg/B0qnKMD50yt8fisBcem1TpYM3EJruweMVx1iC9yeKkI/gmboRRykHaVh8RfYnyi59xW6lyKaUM7frP-PBZBtf-A\",\"filename\":\"Leda.jpeg\",\"size\":55738,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/tcIGATPcRCtd049r7Fgp8w/I6dEKWBru9yfxlxuj_jqS6VPM5tPfH9uRpGurx433w-Jh0OMs9hxDYaf0Wts4x5w/36NmEidlcDpfUY0GeXz-T6ZisElY93scpdwAE9E-60c\",\"width\":47,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/FY6yDustWEFhEanfHsLiew/lUGCZ37OrOv4eOHs54B5cyhuH_KYZ_sRJj3xJm6agAwKLFHJgp9ENUAUr_aRvy7p/7fkbx5q18DOhGfu4Y9qYitHak97hFc95Ia4KJn7pOdo\",\"width\":512,\"height\":487},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SkqyeYlUwvB5eZ3HXR-WnA/eQ8kVfyMfOIAe0nt0AAr5JZsVXI5VH_-adl61797jUvGqwOWHi1gxwP7Djcrq7jZ/R-xpUZPD_Vg0AtteQyE7mdCepKsgDoC_PMC0pBIdbew\",\"width\":640,\"height\":487}}},{\"id\":\"att4MPT0gu8r2DZdv\",\"width\":414,\"height\":526,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/51PgVPyCGs9cVJ3cQrl2Ug/hdBqa5gfOh-r8Yu2stxyDZeWe81XA73L_2ukySRV0vt8AAITDuqShheUTfqTI7by/ueYumqVUmoceXXdGtVc2CM4KpVaFkUGjTCO665HMIsA\",\"filename\":\"Mother_and_child.jpg\",\"size\":38679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/MtJhRG0v5fRAudhm9d08BA/yGz0vfgYAaOIWEjS05aYhCFxx3spVTaN26Uvk9cclgUpBAnYKsKjaMlao00H4GwK/kV_2Z1DQjkNyWPm11XY0wEh7833feYTLYy9NgIKOHlc\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/3iPX5rDYNX2yjVMVFOiNAQ/BMLbYQ7YZXrOwP-_II-q_YIZN-Xx0sxaTcwKRbZiMy9X-H6Nb-wD-RopRg9Z4kya/u9doI4szFDL5QiVoySyJjiEb21fjHM5jIgTndoNPpzA\",\"width\":414,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/FZpamV28VPZlHwxYJynzWw/FE1oFtsXaDeo-J2PAkYutSK_uy24F8_dPl6PbAYevqnod2cgkDgKkcaJbMZMmjN8/dEhWCJsVGvbjFDlXnD-wYznKGZfbajhFJuu2B7zaJeg\",\"width\":414,\"height\":526}}},{\"id\":\"attsGNtljepdSppj3\",\"width\":3694,\"height\":2916,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/Ftth_9EuxvxiI5-ajUwXfg/HaDXZZEzCUcha9iCBnZLNwtuzzlHgzRTAheCblvaqy_CGVl_zfrAdX-krAdCabbp/lzeXAWa3IZAXAhOPeYUCGv9jrQL0IjxBMyXcUkk_gNE\",\"filename\":\"Sky-gate.jpg\",\"size\":10002210,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/OVEAOPnFBwtXX39PMRBoxQ/wx8RKVTufdxjUxK-Rdt38uEnAFOw1psGvZe9ziVOuhy1bm4mylgagTg_WCWVlUj5/u7apZWP-lUUEYvVdeLmt1WOabEyavC3Iiz67bNxSdhM\",\"width\":46,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/oXeyWhBLKPtLV2SovYvhDg/em_SHCIgPlZ_7lEskbmx4va9N9cs6U31V_0tlCL7eDUWCMpGF8prb6DI2IybEcct/bVVPoQnKz3QyHtmWIUkerSngZ8CVL_FLEnpfrSgLEEo\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/huhEWbq0uv4l6uT2ZGTmIQ/yA5_kFFaOV_EoqWPeUtfQiOfczXanGWKngCpH2s1cicdU7Jz2zwxrmdbgw52Ep2e/bzJ5DVIXESFMjlOlo1xlUmG4fZvQ4L-yr7lunFYz27s\",\"width\":3000,\"height\":2368}}},{\"id\":\"attpYKQ4gldei2tWd\",\"width\":960,\"height\":696,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/s2rv-ANkjuWLxNdtkdGJNQ/6JGHXQ6TUyOSYumzOizcScoSE2hX2gg7LwAd29qzKEsQ9n8-Qb68UeLxQG3_GfgA/otTB09NGjwcmh3rBcPDsMR-fh4zMpMu0gcQDYW1Vbb8\",\"filename\":\"Akari_Lamps.jpg\",\"size\":110954,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/djPhtv_ejF22xcX1z0mNvA/oAX9mj6RbrRHZNCvdeI_CvrXj6vRGzlwEyc0gIAZTzFLjEk_4BkCmBrZtKvAuDXr/rq-lXJ7CMauoDxUPdSarlQh9Fst1WgcQILOSbZMKXqs\",\"width\":50,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/74mM6R6DzwgeIevmq5WDRA/9DBMA-vNMgNXYW-bKTZLsHpdU8GthktZGdWxnzWLfaat89-jSmOkHL-qAp-mxwCB/BJLkeym_qhZ80T1-n2t8BORzQc-w5pom1qIexweDKBc\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/SEkawiI7aELOLkbNg2Q3Mw/fxMARKfRiUqBGGDzW3YXZTzkSuZalxiKjpQPSA0hEIWaSpadIiFWH0ax5zICYte-/v4TzLvQp50PzBv7JS5W2dlIpEInUH4af6Rks6_8EBoE\",\"width\":960,\"height\":696}}}]}},{\"id\":\"recneNPDcZsNQDxsb\",\"createdTime\":\"2015-02-09T23:28:08.000Z\",\"fields\":{\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Thornton Willis is an American abstract painter. He has contributed to the New York School of painting since the late 1960s. Viewed as a member of the Third Generation of American Abstract Expressionists, his work is associated with Abstract Expressionism, Lyrical Abstraction, Process Art, Postminimalism, Bio-morphic Cubism (a term he coined) and Color Field painting.\",\"Name\":\"Thornton Willis\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"att8enrlgYD3FiHXB\",\"width\":433,\"height\":550,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/KWdoKCxm4y8s2WiPFpm-Uw/XTUrKHbXSeY8WR5Um4GF59YtOvPnvGMeDAzdW9FAZ_kCJYyMNsrkypCnnlIZbdBF/QvzAi9Bihs4loRLRIVnAp48nQN1swxriEJ24s9kXEEI\",\"filename\":\"Color-Drawing.jpg\",\"size\":374784,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/czhQY_26IFtxy1Lidt-Auw/vZ4yFdM6GCBjutVDPIEgAhJhMX28Hb9zFjbCLzIQc3fgpFii_1PMpprFVW3AOHT6/yYHgxj_oFZUVlkQq8Y8-SS8AI3SGXf683LQwJfhoaCc\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/dsZLffyAZyDA4qgP3NFfDQ/CPLiT7xKk-geAxzFfHHjrCX8J6Q6GCjei_GkQDx-N2CH_hVqddvBN51eg2TM9Y6z/L2Uq8IP0VwiHZNh_ge4g4PvhIxPC2wVOURqMe5jFD5o\",\"width\":433,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/9Nj4GNJ4Ln7dQ5bDBg6teA/1G1EWOawzl52dxcZ0ZqdXj51NnS--VG1tUAC1RACPiReSATkh1UohDyQIOsPFBif/LtooRMhp_rf5bmYMujaYw4ZAtkh5WrsAl7R1szn-VTA\",\"width\":433,\"height\":550}}},{\"id\":\"attqEXWllptvGjPDQ\",\"width\":551,\"height\":700,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/8WhbOs4wLfLxOduGk3Hdwg/RBBy3n2CymcDhTHou1KWN3TrajCw5qvkC-JnOduzO655dxdtco0cWHEax1RMfxYd/2QT2uwcb9dGRHhGnKKXB1THyBlkyYxhOiT8ju4JnNjI\",\"filename\":\"study_No._2.jpg\",\"size\":109204,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/uFKpLjwhceIXY97EyzXrIw/youRSRzTXLOWUOc5LOibm72i1dPr9ZUmrRGEgFvY5O_ubzXYwaeHBirqBGYRh-l0/KdzIE5mGQuMS6eXJr8AjL4g8LN1LAx0otcT4_AKh_BA\",\"width\":28,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/_Rw2EmwpdDevqyuTNm9CkQ/q6xmqqMugr2hQ-ylejdr3doUCXtUqf6KVgW1B7pAoMPF24JnfLKS6fykL0PnqKV8/-3IswgdkWyeAZZxvHflGWsh8lqjpVtZRaRg1O8mlIgI\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/XLyIGTzXWhyJ29uQZfcIrQ/Wpaj5c0VGVnoS54IIEVY5vKdepNv7FWa0hrIEV77uIIVtzCAPkt8j2hPV727VrCL/KXGP93BV9T5jzRZX76fsm7t9sXpo21rxpSv3IdxGB3s\",\"width\":551,\"height\":700}}},{\"id\":\"attHI8lwWwEoooJ24\",\"width\":890,\"height\":1200,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/Rm_bE9B8i0SUmMC4WvlNyQ/692NH9OgLtaCNctVmlUDJAwrPaDqmr7kpleVQLmSip38_eeLpnCdtp7rsGc2GV1-/uagAewbU3ZW3pTw9Rukdqx9FwTXPl7VzTzdyDMfrThc\",\"filename\":\"three_gray_squares.jpg\",\"size\":109091,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/kB5Z-580aJc1Cbe2hNmkYg/isckY-yzVxQ-SlTNBhiq3nMX-X9UcXZ-G3p8q7ofepF0bCTh_KSF3MKiSDpRg-0a5wiDfdCbm2QArKUAWrnN9g/s0ty9NTL-S7PLtCTWpCmYlyRUFUHlTJUyglMj279VUU\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/1OSOR9QL17-5w1PW1I8yDw/AVxljC3bli3oKCg6fnirglEwRsZUfzT3ZnMCFzZjYivTXoM14M8oRhfuzRO5WD3MKXcXRPRx0Mul_Dag0UoajA/Lt2irLp8JKbuwGy5pCcPrZ-qYps4ENqsqJKUD0bO2Ok\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/h_B2VlIfZiD3ShvGutHLQA/ncLVh2A0QCWECdE7fJAkcHKai9jyFefORDeV03V0b6LEJtEC4eq_Ca7jc8nV8xfsDZARy5xiORR5NfVPZNuUOA/-vEEMgK82vPRegfCW3qtWNFgDKrFaDb1cgv3Lj5tyYU\",\"width\":890,\"height\":1200}}},{\"id\":\"att5TIJ00ppiNYQm3\",\"width\":639,\"height\":517,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/kT7-7VdbfWdUMCdmM7SFJg/DNZMf0NqZlKBOCHiSLFYWyH9fWreuZHt0Q_W5Zr2NS2BhcLIvUBMTV_xaDBilUpZ/JUcjEo2E0gNIWZagsldiHiFB5dqBX4gKriTTWEmDNJc\",\"filename\":\"Streets_of_Tupelo.jpg\",\"size\":27664,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/rlzUfZodgu7yYNbnkEN_Mw/osn2PXiWZhtgVelrPlyjJXTg9DBaCniYjcmn1PjYmphzHjnXAqqY_rgC2huIL6EyY0N2HLbxT7W45qI80KfD0g/tycfOXm3Xko4Ch7sCU09bDM_sh657GyAbYdMXPm3Ixo\",\"width\":44,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/1a2gQ4dFqrQIWLO8jvoFrA/jjwj8cZYUdOjI9IBsgZzTzt-I3M1A5rg8tZMxFTguA0Qw58qOV1fu3-wnrIPH6MsmjRUYU72caBe1tYlWbQenw/0-LVgkkNXXy_7sZuFOxi6XgTWDBVqWvwAPsPyz518Hw\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/p6yp3J4OHC-b0jTRr8zJiw/vGp4VVMUTdU4i4Bmwgi5h3WYCFVH8NkzEV64ELKs3Kq4PKOch2Z54t7fvT36ad_J/Jy11a4KQhBFfceF0HjuXxRbI1G_g34V90sJ1cXNyQsI\",\"width\":639,\"height\":517}}}]}},{\"id\":\"recraBPRF3m5Te5Hn\",\"createdTime\":\"2015-02-09T23:24:10.000Z\",\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Mark Rothko is generally identified as an Abstract Expressionist. With Jackson Pollock and Willem de Kooning, he is one of the most famous postwar American artists.\\n\",\"Name\":\"Mark Rothko\",\"Testing Date\":\"2020-07-06T17:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attONu0jXlWNlHOxh\",\"width\":213,\"height\":260,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/WUedqKp4Uo-4b5JgO9CImg/R-GZBZlTfiz2VVYaTEX8tJHvdmVNpzDA7n0YCfLwQ0LsuYeWEiod2FC3OH9rpIGJ/Zvw6XnK4MQdZkDq2mwq-pVef3Fzhdf__sMalIHxC2Dc\",\"filename\":\"No._18_1951.jpg\",\"size\":7416,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/kyVnO7KBFMekgVwu1LmaBA/TzeTJYW0rVdR4VEbrYyrHI3eBSC9PT1mP6tS73y1LoaDRH4DTB_EIXcmQ_f_Be7H/1Go7D5cFgkuvnkWvQ5S3fmaGB_Noj7UGbaZMr6zRIio\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/qM_LPi7YoaaqthYtFFcAJw/__jb5iWqwrDF_HhUk4MaWF8JdOHPkt08XLcowNd-EKgMG9gagWh64WKZWbyK5khv/a6ciVsIOAc2YPftTF3vmqZA4edsar-vbMOnlQyaCQpE\",\"width\":213,\"height\":260},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/EAHd2no0Ii9LBOUtW35aJw/9J30QzDbLzmoT-a9_o3mT-rujJN28EUzrEGnOApSN0H4oCAMiLBisbEs0GE8ZSjg/Gr-4ZVJ8HemElZN2RfhVj3ZMoOMOqYcymZkpltnJPyU\",\"width\":213,\"height\":260}}},{\"id\":\"atteYo0fXP5bOpxt6\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/q3qLrWlgBMwbXpgO-ZJrpA/eQyx_yxCIVpQZidmzsfL3A7nIZK5cQX2GyyZ5Hy_HmO3RB-5r99_QuoTxFFZcU_O/JuBULFkwI2j7u4PQouawxMqdA8RGYKrJ3ms2Z9Z_gaw\",\"filename\":\"Untitled_1954_RISD.jpg\",\"size\":23636,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/yOZAxNCR7Gp28mrH7RQLLA/LR_gsYf-4vIgFTidGADDv-YOMhHrkcRqHhlorTe_8evAb58OsxyLyFJN8C0oVx8VOHKunFMwwVWgiXyGM_4xBw/oqW30nbYeKjb8kM-OYx46m09PCoRNyBf0qSaWaUOAGM\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/4JgQ_NwFxn9tvjkS6pQO_A/RBpdA2NbZmL4J16nQsK_SuM0vxi1ApeMFJMID2_77Vk4TjEb37y5suPXjwC7tOb_DPXAZ6pmpi3XHsDVXU0FhA/fIfxdTqmVg_uCbT4G972Sx6s2nH856JDv9Jsj2NeoZU\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/fV8ZHupTtNDQvZ-fdyQZkA/gMK5YwjBxshziNQIOSbNu8vPvOOqxglF7EyJX7u5CuH-9apxKphkl7Z6a30Gy0RDER9jGoP9GeICKnF78APRVg/sNgYpBVg7mzHGdSOKfMlk4Z-eWfCKpXeiW3rHQC48D8\",\"width\":385,\"height\":640}}},{\"id\":\"attneJn9hR5DtDns9\",\"width\":385,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/JKQiBN8LEd3JwIRbAaXcPQ/C2mSmQSZMrHw4usEbNBmQUOfVruIPhl35gIXfhLOfhfEhNONbERwXTtcXBxHHuqv/HwzZe5vGru4OAnp0Vw6J-KbMwkiXEvR6u68idkmgqKs\",\"filename\":\"Untitled_1954.jpg\",\"size\":33352,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/lJaVOiIEptUjq9f6EGbmUQ/4xJ1eUMVwgXXSeZ-Rd0xtJ96532Iz6oCmO7gk8oLGdUGIeu_aGpuRmkBHAl9UBzu/uZ8oIHgpPanHI8Vor4glOhGvHgfcCpQTs-F9otzCTm8\",\"width\":22,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/8sECtQxPCABPMAAhmkqfMQ/8Y4sqejSEtY2WkzXpEcJrfT-JrQoMYr1YGq6Qg5_wJ0JFU9VzqKX3iqu0NYUBpVQ/jShMDjudoYNguwxZXfhBGP7H0QmigWNYxE7aGF1wtvI\",\"width\":385,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/o3tI2plnsQ2JhzT-hfoGFQ/TP82bV1NmLRR1tayhzdfEb-jPjrnFbrQDFVLEzKdpcwhpNg5tifnxAV78YHG-f27/NsPptxPCk2UlFlAyuMEg7vLCzvif_0v0X4lWWUoAcek\",\"width\":385,\"height\":640}}},{\"id\":\"attnicjT3NIYNL5Le\",\"width\":551,\"height\":640,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/erBHQ4dRNKDTHuEujhXebA/-oyJzObvNbFooW9MHW6cJeSGI1LGxZmVdTq0UMoXKUBXnw32q8yjIbVoPasFwqGq/NFCNSQJ6QNwBOOlYyrIt-u0EpU1brtX1ZlUPAZ7zngc\",\"filename\":\"Untitled_(Red,_Orange).jpg\",\"size\":43346,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/H7MmGRbqm8vb5Vhlk4nwMQ/hRytvhd_xbHR0B9DtcWU-sW9IV23Mf9LetVEjCc6lOqMsPoQAy02lFlPgJI47akTEUmwNpUDkW-1Z9A_VAFfng/u16dGBgRAOiYlsD3tHDsK_bh53TmuY3TxNWO1mWuMq0\",\"width\":31,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/BpJUnccMLuE-UdU7cBnnoQ/6z4Kl7I5JlaAoVhVjECeVdBoBv4mBAyd9OvhsWyNIjBiw2RKEo9raaCf_W7-gaV1bXcCSHJsFH_zUblH8C3jZw/ecQvkPDqdrk4BwCmaKKRjAoy7UqRYNQsLbkBFqURtBc\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/xMkSVSWV8PhlZFq0fAw6zw/E0DY12J7t7OUjSJEGtYFw-Ujvzz-WM11mFxHdE4c-JZPd5-BhgYys0Dkb7x7qwjWtSsrOLm6vTuCEylb2n8iwQ/pCCYPzqym3j1KpeiOWj3mzKoABP0DqvIPHYIi5Rxsvs\",\"width\":551,\"height\":640}}},{\"id\":\"attpDkpjaf734NjM0\",\"width\":480,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/Gv2Cc3nYSpshAgJUbnME-A/iv2F34x8Us5b1P2JGKKuFAnzu-WH56MIdnUDiMpWNsy6RspJxDdPIvGVWUQjBdcg/WKyiNNUqBoSg5RMAQ5vIkmHrkZMs8Fm47bBFEE668J4\",\"filename\":\"No._61_(Rust_and_Blue).jpg\",\"size\":35819,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/QeG1msQNQeMKhCcWIS_dSA/7elM1lSWuBARAAndfymtyd3Z4_WAbsZEiEkQHa0oLD_XDuspSmA_jPFSlGTN-Y3yJKB3HHMhFWycbbeRP727xQ/CsmS1tSif18xWjXfd_JJK3P6xMFjxuooGpQpfEfB77k\",\"width\":29,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/ra3CUzXKorG-O4FBrM1qwA/kIzpNwgWIxh7azh7IkGZeStuB4cpjS5DJMOgb-hkSChkaEHcFFUiJ4epeF7PB76pZxWLgBxNbh_FGJ0vSkCzHA/Fo0sK1ioJA-6o2XeNOFqxZyMb7sCeOJooVqoYcx6fak\",\"width\":480,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678183200000/__iJGUe7P3q5yPJXEyViZw/LDS12utWeVahzToUvycMiI4_-MCsyCClsO4thJ5NAynCrJMWNojygIDIuAgEMX5QD0JykyFHvakSZb5a8BVQNA/iMFTxlWD3zVae18vl4X1o1H7xiCWvEYWXp6cmMD3p6s\",\"width\":480,\"height\":600}}}]}}]}");

            string bodyText = "{\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/listRecords",
                    HttpMethod.Post,
                    fakeResponse,
                    bodyText);

            string errorMessage = null;
            var records = new List<AirtableRecord>();

            Task<ListAllRecordsTestResponse> task = ListAllRecords();
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count == 7);  // 7 is the item count in the Fewer Artists table.

            List<AirtableRecord> doubleList = new List<AirtableRecord>(response.Records);   // Double the size to 7 * 2 > 10.  This will fail the test intentionally.
            foreach (var record in response.Records)
            {
                doubleList.Add(record);
            }
            try
            {
                Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task2 = airtableBase.CreateMultipleRecords(TABLE_NAME, doubleList.ToArray());
                var response2 = await task2;
            }
            catch (Exception e)
            {
                   errorMessage = e.Message;
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                 Console.WriteLine(errorMessage);
            }
            else
            {
                Assert.IsNotNull(errorMessage);         // If not error, fail the test
            }
        }


        //
        // NOTE: We cannot have a test for HTTP status error 422 for Invalid Request because the AsyncSend() we use
        // in this test suite is a dummy one. It does not send anything to the Airtable Server.
        // The purpose of this test suite is for testing the sanity of the code in Airtable.net but not of the
        // Airtable Server.
        //


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzFAtApiClientCommentTests: Create, Update, and Delete
        //  Create a comment for a record knowing its record ID
        //  Update the newly created comment and then delete it.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzFAtApiClientCommentTests()
        {
            // Createa a comment
            fakeResponse.Content = new StringContent
                ("{\"id\":\"com4M8tyislwDZWVU\",\"author\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"name\":\"Ngoc Nicholas\"},\"text\":\"Hello World @[usrvQIoafw4agP8m3] @[usrBw9fdcVFbu7ug9]\",\"createdTime\":\"2023-03-07T01:15:59.529Z\",\"lastUpdatedTime\":null,\"mentioned\":{\"usrvQIoafw4agP8m3\":{\"type\":\"user\",\"id\":\"usrvQIoafw4agP8m3\",\"displayName\":\"Ngoc2 Nicholas\",\"email\":\"ngocnicholas@yahoo.com\"},\"usrBw9fdcVFbu7ug9\":{\"type\":\"user\",\"id\":\"usrBw9fdcVFbu7ug9\",\"displayName\":\"Ngoc Nicholas\",\"email\":\"ngocnicholas@gmail.com\"}}}");

            string bodyText = "{\"text\":\"Hello World @[usrvQIoafw4agP8m3] @[ngocnicholas@gmail.com]\"}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/rec6vpnCofe2OZiwi/comments",
                    HttpMethod.Post,
                    fakeResponse,
                    bodyText);

            string commentText = "Hello World @[usrvQIoafw4agP8m3] @[ngocnicholas@gmail.com]";
            Task<AirtableCreateUpdateCommentResponse> task = airtableBase.CreateComment(TABLE_NAME, AL_HELD_RECORD_ID, commentText);   // record ID of Al Held
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Comment);
            Assert.IsFalse(String.IsNullOrEmpty(response.Comment.Id));
            string IdOfJustCreatedComment = response.Comment.Id;

            // Update the newly created comment
            fakeResponse.Content = new StringContent
                ("{\"id\":\"com4M8tyislwDZWVU\",\"author\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"name\":\"Ngoc Nicholas\"},\"text\":\"Hello World updated\",\"createdTime\":\"2023-03-07T01:16:00.000Z\",\"lastUpdatedTime\":\"2023-03-07T01:24:45.528Z\"}");

            bodyText = "{\"text\":\"Hello World updated\"}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/rec6vpnCofe2OZiwi/comments/com4M8tyislwDZWVU",
                    new HttpMethod("Patch"),
                    fakeResponse,
                    bodyText);

            commentText = "Hello World updated";
            Task<AirtableCreateUpdateCommentResponse> task2 = airtableBase.UpdateComment(TABLE_NAME, AL_HELD_RECORD_ID, commentText, IdOfJustCreatedComment);   // record ID of Al Held
            response = await task2;
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Comment);
            Assert.IsTrue(response.Comment.Text == "Hello World updated");

            // Delete the created comment
            fakeResponse.Content = new StringContent
                ("{\"id\":\"com4M8tyislwDZWVU\",\"deleted\":true}");

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/rec6vpnCofe2OZiwi/comments/com4M8tyislwDZWVU",
                    HttpMethod.Delete,
                    fakeResponse,
                    null);

            Task<AirtableDeleteCommentResponse> task3 = airtableBase.DeleteComment(TABLE_NAME, AL_HELD_RECORD_ID, IdOfJustCreatedComment);   // record ID of Al Held
            var response3 = await task3;
            Assert.IsTrue(response3.Success);
            Assert.IsTrue(response3.Deleted);
            Assert.IsTrue(response3.Id == IdOfJustCreatedComment);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzGAtApiClientListCommentsTest
        //  List comments of  a record with a known record ID
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzGAtApiClientListCommentsTest()
        {
            string offset = null;
            Task<AirtableListCommentsResponse> task;
            AirtableListCommentsResponse response = null;

            fakeResponse.Content = new StringContent
                ("{\"comments\":[{\"id\":\"com8pc4Ht1i4CkPM5\",\"author\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"name\":\"Ngoc Nicholas\"},\"text\":\"Hello World @[usrvQIoafw4agP8m3] @[usrBw9fdcVFbu7ug9]\",\"createdTime\":\"2023-03-01T00:14:37.000Z\",\"lastUpdatedTime\":null,\"mentioned\":{\"usrvQIoafw4agP8m3\":{\"type\":\"user\",\"id\":\"usrvQIoafw4agP8m3\",\"displayName\":\"Ngoc2 Nicholas\",\"email\":\"ngocnicholas@yahoo.com\"},\"usrBw9fdcVFbu7ug9\":{\"type\":\"user\",\"id\":\"usrBw9fdcVFbu7ug9\",\"displayName\":\"Ngoc Nicholas\",\"email\":\"ngocnicholas@gmail.com\"}}},{\"id\":\"comThy6KTVRbi4SIa\",\"author\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"name\":\"Ngoc Nicholas\"},\"text\":\"@[usrBw9fdcVFbu7ug9] and @[usrvQIoafw4agP8m3]. Also @[usr01234567891234]\",\"createdTime\":\"2023-02-28T22:25:24.000Z\",\"lastUpdatedTime\":\"2023-03-01T00:56:05.000Z\",\"mentioned\":{\"usrBw9fdcVFbu7ug9\":{\"type\":\"user\",\"id\":\"usrBw9fdcVFbu7ug9\",\"displayName\":\"Ngoc Nicholas\",\"email\":\"ngocnicholas@gmail.com\"},\"usrvQIoafw4agP8m3\":{\"type\":\"user\",\"id\":\"usrvQIoafw4agP8m3\",\"displayName\":\"Ngoc2 Nicholas\",\"email\":\"ngocnicholas@yahoo.com\"}}}],\"offset\":\"MTk1MTE0Mg==\"}");

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/rec6vpnCofe2OZiwi/comments?pageSize=2",
                    HttpMethod.Get,
                    fakeResponse,
                    null);

            do
            {
                // Set pageSize to 2 so that we wil get one page of 2 comments and one page of 1 comment.
                task = airtableBase.ListComments(TABLE_NAME, AL_HELD_RECORD_ID, offset, pageSize: 2);   // record ID of Al Held
                response = await task;
                Assert.IsTrue(response.Success);
                if (response.Comments.Length > 0)     // This guy has 3 comments and all of them have @[] patterns
                {
                    Assert.IsTrue(response.Comments.Length == 2 || response.Comments.Length == 1);
                    string textWithUserNames = null;
                    foreach (Comment comment in response.Comments)
                    {
                        // Each comment has its own Mentioned which is Dictionary<string, UserMentioned/UserGroupMentioned/UserEmail>
                        textWithUserNames = comment.GetTextWithMentionedDisplayNames();
                        Assert.IsTrue(textWithUserNames != null);
                        Console.WriteLine(textWithUserNames);
                 }
                }
                else
                {
                    Console.WriteLine("No more comments.");
                }
                offset = response.Offset;
                if (offset != null)
                {
                    fakeResponse.Content = new StringContent
                        ("{\"comments\":[{\"id\":\"comP2byhUHWfrEzZN\",\"author\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"name\":\"Ngoc Nicholas\"},\"text\":\"Test comment for Al Held, @FooBar@yahoo.com.\",\"createdTime\":\"2023-02-28T22:24:57.000Z\",\"lastUpdatedTime\":\"2023-03-01T00:51:59.000Z\"}],\"offset\":null}");

                    fakeResponseHandler.AddFakeResponse(
                            BASE_URL + "/rec6vpnCofe2OZiwi/comments?offset=MTk1MTE0Mg%3d%3d&pageSize=2",
                            HttpMethod.Get,
                            fakeResponse,
                            null);
                }
            } while (offset != null);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzHAtApiClientListRecordsCommentCountTest
        // List records
        // Returned records do not include any fields with "empty" values, e.g. "", [], or false.
        // Comment Count is returned for each record.
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzHAtApiClientListRecordsCommentCountTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"records\":[{\"id\":\"rec6vpnCofe2OZiwi\",\"createdTime\":\"2015-02-09T23:24:14.000Z\",\"commentCount\":3,\"fields\":{\"On Display?\":true,\"Genre\":[\"American Abstract Expressionism\",\"Color Field\"],\"Bio\":\"Al Held began his painting career by exhibiting Abstract Expressionist works in New York; he later turned to hard-edged geometric paintings that were dubbed “concrete abstractions”. In the late 1960s Held began to challenge the flatness he perceived in even the most modernist painting styles, breaking up the picture plane with suggestions of deep space and three-dimensional form; he would later reintroduce eye-popping colors into his canvases. In vast compositions, Held painted geometric forms in space, constituting what have been described as reinterpretations of Cubism.\",\"Name\":\"Al Held\",\"Testing Date\":\"1970-11-29T03:00:00.000Z\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attCE1L8ubR6Ciq80\",\"width\":288,\"height\":289,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/cLzPASbaf-nhf0UfjMacog/5ZzgWwVWbZnjCK9e7QPrcwnasstesEAxOOSpdVHw-NCUmnMnhP1rAeKoiTKe8QUP/XIXlu3ElxqKk51hWBuABlWSjBBl6T7ksD_eEwggKJ3Y\",\"filename\":\"Quattro_Centric_XIV.jpg\",\"size\":11117,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/tlqMe4shNaYUDtc5S_Y8Tg/_U6I7kyOn6rcJxu7jFiY8octZRECAJeBFB1eeT4l50bS8eruSJPUc0qYHNgpA4yGhp7FL0A9dMyH8hoQDejMTg/oDAROzKuQ2ABMUndA98XWOPELNtYESfi2uSEHZoZu0w\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/HDgWDJvJNdLzMzYkjqbW2A/nPthWm8tI7TSOPRhUf8GuPEXtMDAyBpFpvrQyC3y2E5Mpg6S-CV1sanSjyRAn-TETkBpJhV64pyeGEzkz7Mi3w/TJrinb5k_HQCEpH7PmEokol320pyCHvDqsA7FxsBfDQ\",\"width\":288,\"height\":289},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/wP1BHoK7YrIXd7OnJdpnSw/OE_Q71YL8tBB7ZjCbk_j7IKJu7EPzOHLD3XFcyan4ynkHSxtJz5kKbFtMrz_zTLX9unz1xRqHRD0Hw6KtrduVg/7ksoo9_nDLyG9YZzEMU2-EUipHqnpERY2RRts3umONA\",\"width\":288,\"height\":289}}},{\"id\":\"atthbDUr6hO3NAVoL\",\"width\":640,\"height\":426,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/6uSGZ3L7CwDmn81y4fV47A/HxMfXfYFw-q8F5wMfBH6E6F07GBwfQA5rRZ5IxaoPaBU0-woASJb1HyObbOdnkbW/xY0We1wUPocACDziCd7dMJzY-laE37M1vLCA-yp8Td8\",\"filename\":\"Roberta's_Trip.jpg\",\"size\":48431,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/N77JSAtZkMg9YOvf19N4MQ/tDpZeLtIefltlndON_vFBFB3nbHO2mVD_n2MQoiBmsmbKYWrV36atwzTvzpIJHL9/oZt34Izfl2t94TtpapfEoIUFpSuF21BPXNUDcH7qC7I\",\"width\":54,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/vREz0T6TCXKeleSBkwtj3w/6ZX5UYhQMM7O1v_d-sbofbmg6mIzp68gv_pBMr1y1zI2Vl0UfiR6_wGhlcRf7x_p/sPxM5MKHFMiBPTAaIU_B79BqOdsvVpWdU5GURs75iuQ\",\"width\":512,\"height\":426},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/EgS3CLgxyCyNrgH59dAMGQ/DKw_sg0QnZmmRKKTDsA_p_bcBwn-uIXXZuJ5RtgtqnmYr_PYu0GTehOkPxrxD4Jd/9cgpm6cPoqKZOT_Rki6wVhCqCKjK5imOpZ2MW9UFttQ\",\"width\":640,\"height\":426}}},{\"id\":\"attrqLTVTRjiIlswF\",\"width\":640,\"height\":480,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/tpgBurj4DgwM4D4oyMu8fw/33zbSb-H7s_rhimfgPYOB_WtMWrD4my1jSVBKYyr9f0fjUpigvljfon7Fk5lh0vQ/F39PORYT5jJFnaoQgu7H7PoFgaw7b8gi5UU5bi-AoCA\",\"filename\":\"Bruges_III.jpg\",\"size\":241257,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/1FlTXYK1zSIgT1ed5_0_Ow/-N6BTdbnFwDVWDld-GEADenidwgudlq1fL0RFNvw8iHS3RiyTdVgRxrLqv35XzcT/g4yFqPj0vj48YKsgKy7jveWEPXPbDCrqIfv5xLxr_rw\",\"width\":48,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/cJZTwcDd8qionKdANQPquw/6pPDQki9cm8nXHTvItojQaEWpLKZMSC7IA9KFqp6NRO2KCR_9TyxIv5GjedZJGXV/kab-AoRLBj7jSch2eCzMN9Yy0-oekkl0DKZQ7ehAmTk\",\"width\":512,\"height\":480},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/6DjhD_JAywdonP8IOXSEXg/BZARvy-VrGndMjkPS8FLKCWZKEm-iO9o79iKey2_pXCrpgBoPfUMd_-cihSTuLbW/JYdAXuTnCa9un3-lZ0gvPJqumdfxw0KgAPC_7GRBtb4\",\"width\":640,\"height\":480}}},{\"id\":\"attQ4txWAL0Yztilg\",\"width\":716,\"height\":720,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/5pBQrmc_Va3eZ2qZaxflBA/eUHxbkdQnKveKniHv3wCx9H2h4If3-ExlGgOt_TIMfpkaw7Lpm7RtvraY8mXb1ts/gQs8RdaTIE4sWfsG9elUutFUico-SP6_unPver9nx54\",\"filename\":\"Vorcex_II.jpg\",\"size\":217620,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/u7o2Mj2c2E_J-f8QJd3VxQ/SUYEFrt98j2AwEORNqpKjsZ5VoN637Le-tDkYRA1pjo55SI9_1rfRBg2wHTW81OL/R3_h7rJiqbcuAYIWN7GWUWc4Zp9B56-u73INzSHpcfI\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/UXPjRZQDgfAeVOOajonxGQ/1LnzZRAy5htjlLsHIApyG3HSokxQqkilFlUW_0HXgvK_G5RpCRfm_gNDmvpFbq3U/qiIhB0Y0if5PwS9eUWRMlyyHjMGQ1dvBp_5qtQHClow\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/J5L7T7pOiuX1F7ptYR5A_w/r72EOG3S6Z7EJcymnR7B7jcUfcCWMwD7oJ1aTpYYMCKufGzWvGoDNn8KW7Z-mOzO/lJC4BLMLWXcRZMs0yjTceRGO0RK9R9vucl1XsNSoOQI\",\"width\":716,\"height\":720}}}]}},{\"id\":\"rec8rPRhzHPVJvrL3\",\"createdTime\":\"2015-02-09T23:04:03.000Z\",\"commentCount\":0,\"fields\":{\"On Display?\":true,\"Genre\":[\"Abstract Expressionism\",\"Modern art\"],\"Bio\":\"Arshile Gorky had a seminal influence on Abstract Expressionism. As such, his works were often speculated to have been informed by the suffering and loss he experienced of the Armenian Genocide.\\n\\n\",\"Name\":\"Arshile Gorky\",\"Collection\":[\"recuV4lqy2awmYEVq\"],\"Attachments\":[{\"id\":\"attwiwoecIfWHYlWm\",\"width\":340,\"height\":446,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/BkLlGmrRyvPPVe2K9CLs5g/Xa5z1OLqQajIc3s5rUniFlqdJtaRLITP1gDihXUjv9mU-oL-UYW2vch6-LycxAE6/-WytnHsMpCFISk34tpKtqbCgebJHwBskHCmfCQuGyy4\",\"filename\":\"Master-Bill.jpg\",\"size\":22409,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/a1Wo2jwD7Nm02ZkgTecnOQ/YP7vpHl89YZhPK6Auqq7XXdArGgW7P7MzRPK8phuAFxFZ1eA3krZtGvtjgJubKJw/RQDizxuE0ANsusjug0XY5b4bry7zere7SX-kWGadsBE\",\"width\":27,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/VZnCwT403jYYR4RMHBbjkw/FDfTVLEaSRANCkhdcClNBr6hvdcNvBHO8kdqvnipJC7cM6fsAAvG2lk8xCyS2u6p/e--H2BNALoRyyz8YG0G3x0XgU-G7znCqqMrLbBy9HTk\",\"width\":340,\"height\":446},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/1Ligk6DdfkUcIPeYIK7jXw/gluQi7ZEuNJWp8FL0b1vcsmFGiwZ1V8PeyDjTgya4EtbPGtT5lCZI4OzPunRKOh7/wHHIp3maQqjoAx4Vf-KZ-42BD1k6PGxgM_odYe-E13s\",\"width\":340,\"height\":446}}},{\"id\":\"att07dHx1LHNHRBmA\",\"width\":440,\"height\":326,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/yyxzI3AUu_1wrHeDrg01Mw/3vhh48bD9WrbGAy6dPEkLigmlptj4u36J06Tny2NpvGo8hUKjuLWuAFf_zZ1TXHXlVX9Zunx8W2Jpe-An581BQ/MtAshO4rp54zZ8mEc5fSznfz8rXALER1HhxkL693H2k\",\"filename\":\"The_Liver_Is_The_Cock's_Comb.jpg\",\"size\":71679,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/exEDuGNHaN1r7daUPTZZTg/S7T_N-PD9cdjLkNHccfFECVPGYr3CBueReBFWqml5IGeUECmjd3VNHEvNSJ6tuw1izsYWARx1PZOhVTBtHkj2Q/C-qkMHY4IK9BRH2A6Xs4Zsh8_iQ_ZXYF0ZNDL5rGlAQ\",\"width\":49,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/CtI9oKTks-lAyxASD4JvRw/NB-UqZlhnCUgLXlSJGO56SGkj5gMoSicF6KimvzLlZwu_3fbPbVMEQz-IM7u2GJqcrQiTb9cR4vgj2SBsiWLlg/ErWZMqQkeGOKA9KJVdgY9nm-BDC5iJZD8_lKsXtGzmw\",\"width\":440,\"height\":326},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/ModwjU12JeR_gN8z4Tv7OA/2ITtCjxSyXdeVSo9Q2rUwR45vW69jCCqgt1pQChHqh7NQn8gZidlZSK65ZGR15ik3rX0zioErxL9Xmp2M67Vvg/cuuaPn3rJ71NahEKXqbTPWKdx6XYFyAafkCz8mHmFgE\",\"width\":440,\"height\":326}}},{\"id\":\"attzVTQd6Xpi1EGqp\",\"width\":1366,\"height\":971,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/nz6cidGit13btuinqJkSRA/1LNtH-aBONGMEZlPPc1UG1AmZ32VUbnkbQYnHMdv3xx6ZVJ7Y5Xv9Eu4MKO-0k2R/ow2_L8qrL-eSyBjKB9odvpgvML0-HIBmApTJOS4euEk\",\"filename\":\"Garden-in-Sochi-1941.jpg\",\"size\":400575,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/RO3CXNaWJUokuqE7JtzAcw/IGmsxrXEKduthbccTGUdxbgjnS5rklJnFqKMYL_KRgpxQWUryUbZQgR7eXTgpDcd80VUTNduVA2coqDwdaJotQ/XcdwxfaPEXTLdPgwaAtlIsGt6gDNOiweU4jti4Ow5pU\",\"width\":51,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/MHBkPj3Ck44z9y-QDIh_DQ/ApXNgwfXAkstuUME6XF2-Q50FePzBHzXIrjX1wtwlbl6ilJPNn1JXvqFDRqZkWmx6JPmbaXoQ9tNP3emNW-lJg/RIM_EsOf48girgiSQkpMhsZ8CMtQRkRMsqYz2ZfqzKs\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/umTYJ4zt-r_QonDyPYuk-w/AI4noq1xrUezZN7Q33iKwNmOTGSG0QbsnKJPx7JOGpFg284NdNWFd80AcCSmKv8xp1EuM5y-hOtPkn7lcV6n-w/ja5ALfq196NHR8SS1hLAFCyouBbc7JfdJcQaGhJZ2r0\",\"width\":1366,\"height\":971}}}]}},{\"id\":\"recTGgsutSNKCHyUS\",\"createdTime\":\"2015-02-10T16:53:03.000Z\",\"commentCount\":2,\"fields\":{\"Genre\":[\"Post-minimalism\",\"Color Field\"],\"Bio\":\"Miya Ando is an American artist whose metal canvases and sculpture articulate themes of perception and one's relationship to time. The foundation of her practice is the transformation of surfaces. Half Japanese & half Russian-American, Ando is a descendant of Bizen sword makers and spent part of her childhood in a Buddhist temple in Japan as well as on 25 acres of redwood forest in rural coastal Northern California. She has continued her 16th-generation Japanese sword smithing and Buddhist lineage by combining metals, reflectivity and light in her luminous paintings and sculpture.\",\"Name\":\"Miya Ando\",\"Collection\":[\"recoOI0BXBdmR4JfZ\"],\"Attachments\":[{\"id\":\"attLVumLibzCVC78C\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/4h4YOhia5UoxRQyXWlqZwQ/jAS34TE0c05wY5ApMzikxRgKfs9d_5Yt-oiK362RAmGxd4C6hJBqY5lSLIqB_nRd/Gl4fevqda9gebf0lgRBq1f9FNliocdEvNaUjO3_1QOU\",\"filename\":\"blue+light.jpg\",\"size\":52668,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/o-A_Tt41Y72l6olys3Xaxw/ibXswx34OuWkgknAWYSjUug9IPUry6ccekRGay4vV8sdeUcqzEhL6E0dOphJu2SU/ncUWhe1gdtvyHzKZ2u5xFIq_s4AYgYNma3hwOfe7-u4\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/_ysAgK7ufcuNa0963BleZQ/xHXUuhXJ1cgIjdDDyDUVqzj7bDSeURX8xesIrtTgToq-Nm4VPpphVTcKx1xui2T5/6shOi4JaK6vANbyM0AEzqggpb5DI86zxm95JK8oOCQE\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/I8sLQV0paxtyeQv3XVbsyw/z4FgKn4yzmrNMDUgHA_jdx2RSmsatqrD4MbC99SRzcGQVKvnqNm3k6aA1bFq3jzo/PoAPjoM3h8Y70KO-A-oU1arAQ3cjgrME5p3Y8N_JubI\",\"width\":1000,\"height\":1000}}},{\"id\":\"attKMaJXwjMiuZdLI\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/msU9gIldhQwxOtpYHWGFXw/5KY7IuIARWJHPTfCEtL_ua_1eKW5KpWusBocMIMlHjJyv1tIlNjHaSPxeljA82FNztToRReJN_f9eoc0Cdd-Jg/4cui0B_MkRHAmMc0tZapyWgEmideiIEBjAOxmjzpO70\",\"filename\":\"miya_ando_sui_getsu_ka_grid-copy.jpg\",\"size\":442579,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/bJ2ahcTM7zRlFQyh88dmqg/5NGwV-xXWws7P7nE8aZG3EGZEG3guxGShMdmfIaQpHaviMkMTS8YHOIJGLQfG4HjrMrrC7f1eVTntnUYkwSpMw/kyWPT4E1zvitVPP_ZwWkNco4YOulpsEtyRXMlJszBQg\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/lLea71o6SY3tiXC7fjZKNA/CuK5-qrLxQRxJDOUG3v5xyPVD-4ptIg0c12NK7P6LwBXpYBVV2_rC1TIIfExPpnYjXeDADQP4AdXYlbSM9cLZQ/LruLDLQ8JaulX54Wr09lQ0j5UXWHifqmDFC8pTtBpOM\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/WB243jFGguabJkdrwG7s3g/BvcC3XBllFqvPBChPet3eBFFHjDU1FeGJIz3wwVkhDY3iF-pSPwmkiUQgDHM5KqE6tGgfkihzGBmGjrjK4Iq-A/hO7JlLOzJkQXXk9z10XDQbZJlfelbh_wFp4emTvSwYo\",\"width\":1000,\"height\":1000}}},{\"id\":\"attNFdk6dFEIc8umv\",\"width\":1000,\"height\":1000,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/MrKdwR47aaSYD8brzMea8g/XpVUKBxsM8aTd1cMoSg5LYI2zcEGD9ZtsQ4Cywj0kZKFGSeeTBFDKJ4Qxd7PbUiaU-L58y5aFxqd_Hx3lZep0owMVy5Db5t5vtAedK4heP9Z74rsoX2ACGq41oLFmHfOWdWVUv3t6yFNA7rSH5X3Rw/G_UuUaXXzkDgdgpwM-ZI4wS-joN3Uxo3ct-IIyA3XfI\",\"filename\":\"miya_ando_blue_green_24x24inch_alumium_dye_patina_phosphorescence_resin-2.jpg\",\"size\":355045,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/0fK-1JNu0eYGU_snrruq7Q/EIlMflTpOOFmRnW6xJCpSpjP5eRvmviufSNfzPb5nd4HUVuUV0cpX_erSf48uEolHSCOrjw9F-n5oinIrA_lfojYxa0BZ5dQQ8geOiJMUdo39QPD5-AJgt_YGQdnkaIHvtrPbKxyxNV-CFCjqXMAPg/03Up5SfYgmuL6S2YbfOAZ9KQjKPr9JDBz1gKxxapi7I\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/ICEONJBsmSo0KEZbwylxVA/hO85mp6Ll75ftNNsq7JMilKpglncY5KRPed1trzsEpeLPBHnCqNn0F3q9jV0VIfSX22PXh4Cbf6r3Ea5OWVwfOGeAaRAXQwXj-W3X3FYiy3waHLw4pJE1bmuvaEhcUK4Afb6Rf9KsILJdftUiQkApg/LVW1diDZmSRtJN9m7DHtGyfgtWWhuzFx6FYFjcOhGRc\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/kpyltB_xnbey8m1FZZm24A/-khoI9TaP-WZHU3lXz7ngVclAHYkLXoCZIByuzbHYccwAhOQYMiS-RsWJbSqoJq9PKKFyHlLY9ccXkE7Ue9KL1_SEjr0KK4ByNhL0Zw9G7BXtIKV-lj6qAc3txlsRyXXuPcV4-jrwaEwmvX1Ng_SYw/LOikCv3SBiUsMOva4oZp3trgxTa5bmlALerS6-Uh7A8\",\"width\":1000,\"height\":1000}}},{\"id\":\"attFdi66XbBwzKzQl\",\"width\":600,\"height\":600,\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/nX_QE9_YMUsAccWFk3qV1A/BsJoGMlkOvX8_lgPUQJdkXmNxeYW5VdgafXCprWsRDCpNprm9k5LwVSF4AWCObfYB_b_SfZBsauTgsdQ6O7FDHQvhG8xrPJxiySXwnZKm2jAKJ-V7cSTjM6WWvTKrx-ia6tAcY3OsTBhkQ1t3jcyruipSHXabGSJRVjlXw2tna6dBbPyKcqRIWxnzP81qdnE/EuMlFaw915U2Puvzt8jLmY1ie2xobrOLR5__3d-CNvY\",\"filename\":\"miya_ando_shinobu_santa_cruz_size_48x48inches_year_2010_medium_aluminum_patina_pigment_automotive_lacquer.jpg\",\"size\":151282,\"type\":\"image/jpeg\",\"thumbnails\":{\"small\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/oVx8KxYNg4Id3EgWG4p-aA/blsKgnqlAE0kRW4Qk5agEVSSfTI1bBZNi6Gj2DbBGPTRl0gzk9MtXJOPtVaiwScgyU-WF7JK27NbruRonbIJY6FJ5OipV_PuGcs4oooa1O3J8yFgJMOzL3oJM79FrceWhaT9xOBdgs_8lQ-u7UZnlxralIntk_Gyl6n87sWWhe3LBTUcQZH9_jXXMkHFAmUq/JLgALSozOKU7c5zJLMzW040nrnZbdG4OVzh3Q_lrXb0\",\"width\":36,\"height\":36},\"large\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/_Y4QBEcai4IggqUflVBccg/NHYiW7KoT7ZJNej-NvJsQXivBG-k5lWL9bsyHLIrKFAZXIoCoA5PgCzPWTvatoEv9ll7JiAVHHQhZ6n4U5PouRR38CjC89HjXDOONbGFi_9GSYk0A6YWeFmzwoUlN2azrVt8dMSxnEO3vrcUc9i7I_Yc_-M1UVQVtAxZoiFvU9n58mp4nwNg6N4pNNlOqChl/8BJcohpnQjjHju4xUAPSR1e3N-BfD69ALhsABAjnyhY\",\"width\":512,\"height\":512},\"full\":{\"url\":\"https://v5.airtableusercontent.com/v1/15/15/1678176000000/PUVNBYXo_MERyJjjAEXPew/c8p2uVHmz6sva_AQ4CynqTGfSa6Cq1XzKFDYsyLK5qQrKeOQzQHHP0GlBJLZRiketuGQJH0E6XGyl4wjovWHOTn-Mv1x28haZfIvOpdKaKGZk7Q0zkD4bq0P9KzNb1ljB45mF2KCyCG1Vry-FazbRilyou0skwDjo6omN2x4v6M9CpsZgTvT6ur1pzc9UFUv/y6JCD0Xs3QzRMbzxoXPmXKG8VFj_fsXnGUfp1icGQ3o\",\"width\":600,\"height\":600}}}]}}]}");
      
            string bodyText = "{\"recordMetadata\":\"commentCount\",\"maxRecords\":3,\"returnFieldsByFieldId\":false}";

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/listRecords",
                    HttpMethod.Post,
                    fakeResponse,
                    bodyText);

            Task<ListAllRecordsTestResponse> task = ListAllRecords(maxRecords: 3, includeCommentCount: true);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count > 0);
            var record = response.Records[0];                // We knew this is the record for "Al Held"
            Assert.AreEqual(record.CommentCount, 3);            // Al Held has 3 comments


            fakeResponse.Content = new StringContent
                ("{\"comments\":[],\"offset\":null}");

            fakeResponseHandler.AddFakeResponse(
                    BASE_URL + "/recaaJrI2JbRgEX5O/comments",
                    HttpMethod.Get,
                    fakeResponse,
                    null);

            // Evard Munch's record has no comments
            Task<AirtableListCommentsResponse> task2 = airtableBase.ListComments(TABLE_NAME, EDVARD_MUNCH_RECORD_ID);
            var response2 = await task2;
            Assert.IsTrue(response2.Success);
            Assert.IsTrue(response2.Comments.Length == 0);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzIAtApiClientGetUserIdAndScopesTest
        // Get User ID and scopes
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzIAtApiClientGetUserIdAndScopesTest()
        {
            fakeResponse.Content = new StringContent
                ("{\"id\":\"usrBw9fdcVFbu7ug9\"}");

            fakeResponseHandler.AddFakeResponse(
                    "https://api.airtable.com/v0/meta/whoami",
                    HttpMethod.Get,
                    fakeResponse,
                    null);

            Task<AirtableGetUserIdAndScopesResponse> task = airtableBase.GetUserIdAndScopes();
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsFalse(string.IsNullOrEmpty(response.UserId));
            Console.WriteLine($"UserId is {response.UserId}");
            if ((response.Scopes != null) && (response.Scopes.ToList<string>()).Count > 0)
            {
                foreach (var scope in response.Scopes)
                {
                    Console.WriteLine("Scope is {0}", scope);
                }
            }
            else
            {
                Console.WriteLine("Scopes are empty");
            }
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzJAtApiClientCreateWebhook
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzJAtApiClientCreateWebhooks()
        {
            WebhooksSpecification spec = CreateSpecForWebhook("tableData");

            fakeResponse.Content = new StringContent
                ("{\"id\":\"achJjY64Hrd3f2rQh\",\"macSecretBase64\":\"27hPJ71G46eMwCNTLbGjwt4j5JZo/OHgj68+rb2bZ2IwV4ugSNF5BNGKEzpoCnfSBbDDepyjFqfU6AvKBak1tusFaAEcfYrEVeJiJqGZ1CpI0HMp0CnWNKdxP8XQyknpGOL1ruPrVLe2gxMXWNRUHinTz0VIGrewQxGF6lT6jIA=\",\"expirationTime\":\"2023-11-12T06:55:47.872Z\"}");

            string bodyText = "{\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false}}},\"notificationUrl\":\"https://httpbin.org/post\"}";

            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks,
                        HttpMethod.Post,
                        fakeResponse,
                        bodyText);

            Task<AirtableCreateWebhookResponse> task = airtableBase.CreateWebhook(spec, NOTIFICATION_URL);
            var response = await task;
            Assert.IsTrue(response.Success);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzKAtApiClientEnableWebhookNotifications
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzKAtApiClientEnableWebhookNotifications()
        {
            fakeResponse.Content = new StringContent
                ("{\"webhooks\":[{\"id\":\"achfus4RR3IgSeeMK\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":1,\"lastNotificationResult\":null,\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-12T19:14:50.929Z\"},{\"id\":\"achGVe4kJBlLroiOS\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":1,\"lastNotificationResult\":null,\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-12T19:14:51.272Z\"}]}");

            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks,
                        HttpMethod.Get,
                        fakeResponse,
                        null);

            Task<AirtableListWebhooksResponse> task = airtableBase.ListWebhooks();
            var response = await task;
            Assert.IsTrue(response.Success);
            Webhooks webhooks = response.Webhooks;

            if (webhooks != null && webhooks.Hooks != null)
            {
                Console.WriteLine("webhooks.Hooks.Legth = {0}", webhooks.Hooks.Length);
                if (webhooks.Hooks.Length > 0)
                {
                    Console.WriteLine("Enabling notifications for Webhook with ID = {0}", webhooks.Hooks[0].Id);

                    fakeResponse.Content = new StringContent("\"{}\"");
                    string bodyText = "{\"enable\":true}";
                    string webhookId = webhooks.Hooks[0].Id;

                    fakeResponseHandler.AddFakeResponse(
                                UrlHeadWebhooks + "/" + webhookId + "/enableNotifications",
                                HttpMethod.Post,
                                fakeResponse,
                                bodyText);

                    Task<AirtabeEnableOrDisableWebhookNotificationsResponse> task2 = airtableBase.EnableOrDisableWebhookNotifications(webhookId, true);
                    var response2 = await task2;
                    Assert.IsTrue(response2.Success);
                }
                else
                {
                    Console.WriteLine("There is no webhook notifications to eneable/disable.");
                }
            }
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzLAtApiClientValidNotificationUrl
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzLAtApiClientValidNotificationUrl()
        {
            fakeResponse.Content = new StringContent
                ("{\"webhooks\":[{\"id\":\"achHvwpmtKjW9gCBj\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":2,\"lastNotificationResult\":{\"success\":true,\"completionTimestamp\":\"2023-11-06T20:48:29.055Z\",\"durationMs\":183.607139,\"retryNumber\":0},\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":\"2023-11-06T20:48:29.000Z\",\"isHookEnabled\":true,\"expirationTime\":\"2023-11-13T20:48:28.406Z\"}]}");    
                
            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks,
                        HttpMethod.Get,
                        fakeResponse,
                        null);


            // ListWebhooks to see the Notification error
            Webhooks webhooks = await ListWebhooks();
            Assert.IsTrue(webhooks.Hooks.Length == 1);
            Assert.IsTrue(webhooks.Hooks.First().LastNotificationResult.Success);
            Assert.IsNull(webhooks.Hooks.First().LastNotificationResult.Error);
            Assert.IsFalse(webhooks.Hooks.First().LastNotificationResult.WillBeRetried);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzMAtApiClientListPayloads
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzMAtApiClientListPayloads()
        {
            fakeResponse.Content = new StringContent
                ("{\"webhooks\":[{\"id\":\"achHvwpmtKjW9gCBj\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":8,\"lastNotificationResult\":{\"success\":true,\"completionTimestamp\":\"2023-11-07T05:20:01.973Z\",\"durationMs\":18.574319,\"retryNumber\":0},\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":\"2023-11-07T05:20:02.000Z\",\"isHookEnabled\":true,\"expirationTime\":\"2023-11-14T05:10:48.986Z\"}]}");

            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks,
                        HttpMethod.Get,
                        fakeResponse,
                        null);

            Task<AirtableListWebhooksResponse> task = airtableBase.ListWebhooks();
            var response = await task;
            Assert.IsTrue(response.Success);
            Webhooks webhooks = response.Webhooks;
            Assert.IsTrue(webhooks != null && webhooks.Hooks != null && webhooks.Hooks.Length > 0);
            Console.WriteLine("List payloads for webhook with ID = {0}", webhooks.Hooks[0].Id);
            string webhookId = webhooks.Hooks[0].Id;

            fakeResponse.Content = new StringContent
                ("{\"payloads\":[{\"timestamp\":\"2023-11-06T20:48:28.797Z\",\"baseTransactionNumber\":9730,\"actionMetadata\":{\"source\":\"publicApi\",\"sourceMetadata\":{\"user\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"permissionLevel\":\"create\",\"name\":\"Ngoc Nicholas\",\"profilePicUrl\":\"https://static.airtable.com/images/userIcons/user_icon_10.png\"} } },\"payloadFormat\":\"v0\",\"changedTablesById\":{\"tblUmsH10MkIMMGYP\":{\"createdRecordsById\":{\"recmL2nzyOJVrv00D\":{\"createdTime\":\"2023-11-06T20:48:29.000Z\",\"cellValuesByFieldId\":{\"fldSAUw6qVy9NzXzF\":\"Record for Testing Webhooks\"} } } } } },{\"timestamp\":\"2023-11-06T20:59:57.651Z\",\"baseTransactionNumber\":9731,\"actionMetadata\":{\"source\":\"publicApi\",\"sourceMetadata\":{\"user\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"permissionLevel\":\"create\",\"name\":\"Ngoc Nicholas\",\"profilePicUrl\":\"https://static.airtable.com/images/userIcons/user_icon_10.png\"} } },\"payloadFormat\":\"v0\",\"changedTablesById\":{\"tblUmsH10MkIMMGYP\":{\"destroyedRecordIds\":[\"recmL2nzyOJVrv00D\"]} } },{\"timestamp\":\"2023-11-07T04:49:12.530Z\",\"baseTransactionNumber\":9732,\"actionMetadata\":{\"source\":\"publicApi\",\"sourceMetadata\":{\"user\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"permissionLevel\":\"create\",\"name\":\"Ngoc Nicholas\",\"profilePicUrl\":\"https://static.airtable.com/images/userIcons/user_icon_10.png\"} } },\"payloadFormat\":\"v0\",\"changedTablesById\":{\"tblUmsH10MkIMMGYP\":{\"createdRecordsById\":{\"rec9Nb0erkoW9fsH8\":{\"createdTime\":\"2023-11-07T04:49:12.000Z\",\"cellValuesByFieldId\":{\"fldSAUw6qVy9NzXzF\":\"Record for Testing Webhooks\"} } } } } },{\"timestamp\":\"2023-11-07T04:53:20.253Z\",\"baseTransactionNumber\":9733,\"actionMetadata\":{\"source\":\"publicApi\",\"sourceMetadata\":{\"user\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"permissionLevel\":\"create\",\"name\":\"Ngoc Nicholas\",\"profilePicUrl\":\"https://static.airtable.com/images/userIcons/user_icon_10.png\"} } },\"payloadFormat\":\"v0\",\"changedTablesById\":{\"tblUmsH10MkIMMGYP\":{\"createdRecordsById\":{\"recDoe9ZnKJZnbXL5\":{\"createdTime\":\"2023-11-07T04:53:20.000Z\",\"cellValuesByFieldId\":{\"fldSAUw6qVy9NzXzF\":\"Record for Testing Webhooks\"} } } } } },{\"timestamp\":\"2023-11-07T05:10:20.106Z\",\"baseTransactionNumber\":9734,\"actionMetadata\":{\"source\":\"client\",\"sourceMetadata\":{\"user\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"permissionLevel\":\"create\",\"name\":\"Ngoc Nicholas\",\"profilePicUrl\":\"https://static.airtable.com/images/userIcons/user_icon_10.png\"} } },\"payloadFormat\":\"v0\",\"changedTablesById\":{\"tblUmsH10MkIMMGYP\":{\"destroyedRecordIds\":[\"rec9Nb0erkoW9fsH8\",\"recDoe9ZnKJZnbXL5\"]} } },{\"timestamp\":\"2023-11-07T05:10:46.036Z\",\"baseTransactionNumber\":9735,\"actionMetadata\":{\"source\":\"publicApi\",\"sourceMetadata\":{\"user\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"permissionLevel\":\"create\",\"name\":\"Ngoc Nicholas\",\"profilePicUrl\":\"https://static.airtable.com/images/userIcons/user_icon_10.png\"} } },\"payloadFormat\":\"v0\",\"changedTablesById\":{\"tblUmsH10MkIMMGYP\":{\"createdRecordsById\":{\"recmbMlVD0uWUy3Ru\":{\"createdTime\":\"2023-11-07T05:10:46.000Z\",\"cellValuesByFieldId\":{\"fldSAUw6qVy9NzXzF\":\"Record for Testing Webhooks\"} } } } } },{\"timestamp\":\"2023-11-07T05:20:01.877Z\",\"baseTransactionNumber\":9736,\"actionMetadata\":{\"source\":\"client\",\"sourceMetadata\":{\"user\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"permissionLevel\":\"create\",\"name\":\"Ngoc Nicholas\",\"profilePicUrl\":\"https://static.airtable.com/images/userIcons/user_icon_10.png\"} } },\"payloadFormat\":\"v0\",\"changedTablesById\":{\"tblUmsH10MkIMMGYP\":{\"destroyedRecordIds\":[\"recmbMlVD0uWUy3Ru\"]} } },{\"timestamp\":\"2023-11-07T05:30:05.650Z\",\"baseTransactionNumber\":9737,\"actionMetadata\":{\"source\":\"publicApi\",\"sourceMetadata\":{\"user\":{\"id\":\"usrBw9fdcVFbu7ug9\",\"email\":\"ngocnicholas@gmail.com\",\"permissionLevel\":\"create\",\"name\":\"Ngoc Nicholas\",\"profilePicUrl\":\"https://static.airtable.com/images/userIcons/user_icon_10.png\"} } },\"payloadFormat\":\"v0\",\"changedTablesById\":{\"tblUmsH10MkIMMGYP\":{\"createdRecordsById\":{\"recL5S17OeXlIl53k\":{\"createdTime\":\"2023-11-07T05:30:06.000Z\",\"cellValuesByFieldId\":{\"fldSAUw6qVy9NzXzF\":\"Record for Testing Webhooks\"} } } } } }],\"cursor\":9,\"mightHaveMore\":false,\"payloadFormat\":\"v0\"}");

            //https://api.airtable.com/v0/bases/app1234567890ABCD/webhooks/achHvwpmtKjW9gCBj/payloads
            fakeResponseHandler.AddFakeResponse(
                        //"https://api.airtable.com/v0/bases/appvdIxcMzHRMZWUY/webhooks/achHvwpmtKjW9gCBj/payloads?cursor=1",
                        UrlHeadWebhooks + "/" + webhookId + "/payloads",
                        HttpMethod.Get,
                        fakeResponse,
                        null);

            Assert.IsTrue(webhooks != null && webhooks.Hooks != null && webhooks.Hooks.Length > 0);
            Console.WriteLine("List payloads for webhook with ID = {0}", webhooks.Hooks[0].Id);
            Task<AirtableListPayloadsResponse> task2 = airtableBase.ListPayloads(webhooks.Hooks[0].Id);
            var response2 = await task2;
            Assert.IsTrue(response2.Success);
            Assert.IsNotNull(response2.Payloads);
            Console.WriteLine("Payloads.Length = {0}", response2.Payloads.Length);
            Assert.IsTrue(response2.Payloads.Length > 0);

            PayloadsAnalyze(response2.Payloads);
            Console.WriteLine("Done analyzing payloads.");
            Console.WriteLine("Payloads.Cusor = {0}, Payloads.MightHaveMore = {1}", response2.Cursor, response2.MighHaveMore);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzNAtApiClientBadNotificationUrl
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzNAtApiClientBadNotificationUrl()
        {

            fakeResponse.Content = new StringContent
                ("{\"webhooks\":[{\"id\":\"achJei4GOSrTPhsap\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/bad_post\",\"cursorForNextPayload\":2,\"lastNotificationResult\":{\"success\":false,\"completionTimestamp\":\"2023-11-07T20:55:59.869Z\",\"durationMs\":12.422962,\"retryNumber\":2,\"error\":{\"message\":\"The HTTP request returned a 404 status code instead of 200 or 204.\"},\"willBeRetried\":true},\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-14T20:55:31.680Z\"}]}");

            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks,
                        HttpMethod.Get,
                        fakeResponse,
                        null);

            // ListWebhooks to see the Notification error
            Webhooks webhooks = await ListWebhooks();
            Assert.IsTrue(webhooks.Hooks.Length == 1);
            Assert.IsFalse(webhooks.Hooks.First().LastNotificationResult.Success);
            Assert.IsNotNull(webhooks.Hooks.First().LastNotificationResult.Error);
            Console.WriteLine("Error message is: {0}", webhooks.Hooks.First().LastNotificationResult.Error);
            Console.WriteLine("WillBeRetried is {0}", webhooks.Hooks.First().LastNotificationResult.WillBeRetried);

        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzOAtApiClientDisableWebhookNotifications
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzOAtApiClientDisableWebhookNotifications()
        {
            fakeResponse.Content = new StringContent
                ("{\"webhooks\":[{\"id\":\"achfus4RR3IgSeeMK\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":1,\"lastNotificationResult\":null,\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-12T19:14:50.929Z\"},{\"id\":\"achGVe4kJBlLroiOS\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":1,\"lastNotificationResult\":null,\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-12T19:14:51.272Z\"}]}");

            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks,
                        HttpMethod.Get,
                        fakeResponse,
                        null);

            Task<AirtableListWebhooksResponse> task = airtableBase.ListWebhooks();
            var response = await task;
            Assert.IsTrue(response.Success);
            Webhooks webhooks = response.Webhooks;
    

            if (webhooks != null && webhooks.Hooks != null)
            {
                Console.WriteLine("webhooks.Hooks.Legth = {0}", webhooks.Hooks.Length);
                if (webhooks.Hooks.Length > 0)
                {
                    string webhookId = webhooks.Hooks[0].Id;
                    Console.WriteLine("Disabling notifications for Webhook with ID = {0}", webhookId);

                    fakeResponse.Content = new StringContent("\"{}\"");
                    string bodyText = "{\"enable\":false}";

                    fakeResponseHandler.AddFakeResponse(
                                UrlHeadWebhooks + "/" + webhookId + "/enableNotifications",
                                HttpMethod.Post,
                                fakeResponse,
                                bodyText);

                    Task<AirtabeEnableOrDisableWebhookNotificationsResponse> task2 = airtableBase.EnableOrDisableWebhookNotifications(webhookId, false);
                    var response2 = await task2;
                    Assert.IsTrue(response2.Success);
                }
                else
                {
                    Console.WriteLine("There is no webhook notifications to eneable/disable.");
                }
            }
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzPAtApiClientRefreshWebhookNotifications
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzPAtApiClientRefreshWebhookNotifications()
        {
            fakeResponse.Content = new StringContent
                ("{\"webhooks\":[{\"id\":\"achJei4GOSrTPhsap\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/bad_post\",\"cursorForNextPayload\":3,\"lastNotificationResult\":{\"success\":false,\"completionTimestamp\":\"2023-11-07T21:29:08.099Z\",\"durationMs\":15.156026,\"retryNumber\":8,\"error\":{\"message\":\"The HTTP request returned a 404 status code instead of 200 or 204.\"},\"willBeRetried\":true},\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-14T21:12:34.828Z\"}]}");

            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks,
                        HttpMethod.Get,
                        fakeResponse,
                        null);

            Task<AirtableListWebhooksResponse> task = airtableBase.ListWebhooks();
            var response = await task;
            Assert.IsTrue(response.Success);
            Webhooks webhooks = response.Webhooks;

            Assert.IsTrue(webhooks != null && webhooks.Hooks != null);
            Console.WriteLine("webhooks.Hooks.Legth = {0}", webhooks.Hooks.Length);
            Assert.IsTrue(webhooks.Hooks.Length > 0);
            fakeResponse.Content = new StringContent
                ("{\"expirationTime\":\"2023-11-14T21:50:33.685Z\"}");

            string webhookId = webhooks.Hooks[0].Id;

            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks + "/" + webhookId + "/refresh",
                        HttpMethod.Post,
                        fakeResponse,
                        null);
                    
            Console.WriteLine("Refreshing Webhook with ID = {0}", webhookId);
            Task<AirtabeRefreshWebhookResponse> task2 = airtableBase.RefreshWebhook(webhookId);
            var response2 = await task2;
            Assert.IsTrue(response2.Success);
        }


        //----------------------------------------------------------------------------
        //
        // AtApiClientTests.TzQAtApiClientListWebhooks
        //
        //----------------------------------------------------------------------------
        [TestMethod]
        public async Task TzQAtApiClientListWebhooks()
        {
            fakeResponse.Content = new StringContent
                ("{\"webhooks\":[{\"id\":\"achfus4RR3IgSeeMK\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":1,\"lastNotificationResult\":null,\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-12T19:14:50.929Z\"},{\"id\":\"achGVe4kJBlLroiOS\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":1,\"lastNotificationResult\":null,\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-12T19:14:51.272Z\"}]}");
            
            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks,
                        HttpMethod.Get,
                        fakeResponse,
                        null);

            Task<AirtableListWebhooksResponse> task = airtableBase.ListWebhooks();
            var response = await task;
            Assert.IsTrue(response.Success);
            Webhooks webhooks = response.Webhooks;
            Console.WriteLine("Number of Webhooks is {0}", webhooks.Hooks.Length);
        }

        //---------------------------------------------------------------------------------------------------------
        //------------------------------------------- Helper Functions --------------------------------------------
        //---------------------------------------------------------------------------------------------------------

        private async Task<Webhooks>GenerateWebhooksToUse()
        {
            fakeResponse.Content = new StringContent
                ("{\"webhooks\":[{\"id\":\"achfus4RR3IgSeeMK\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":1,\"lastNotificationResult\":null,\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-12T19:14:50.929Z\"},{\"id\":\"achGVe4kJBlLroiOS\",\"specification\":{\"options\":{\"filters\":{\"recordChangeScope\":\"tblUmsH10MkIMMGYP\",\"dataTypes\":[\"tableData\"]},\"includes\":{\"includePreviousCellValues\":true,\"includePreviousFieldDefinitions\":false} } },\"notificationUrl\":\"https://httpbin.org/post\",\"cursorForNextPayload\":1,\"lastNotificationResult\":null,\"areNotificationsEnabled\":true,\"lastSuccessfulNotificationTime\":null,\"isHookEnabled\":true,\"expirationTime\":\"2023-11-12T19:14:51.272Z\"}]}");

            fakeResponseHandler.AddFakeResponse(
                        UrlHeadWebhooks,
                        HttpMethod.Get,
                        fakeResponse,
                        null);

            Task<AirtableListWebhooksResponse> task = airtableBase.ListWebhooks();
            var response = await task;
            Assert.IsTrue(response.Success);
            Webhooks webhooks = response.Webhooks;
            return webhooks;
        }


        private async Task<ListAllRecordsTestResponse> ListAllRecords(
            IEnumerable<string> fields = null,
            string filterByFormula = null,
            int? maxRecords = null,
            int? pageSize = null,
            IEnumerable<Sort> sort = null,
            string view = null,
            string cellFormat = null,
            string timeZone = null,
            string userLocale = null,
            bool returnFieldsByFieldId = false,
            bool includeCommentCount = false)
        {
            string offset = null;
            string errorMessage = null;
            var records = new List<AirtableRecord>();
            try
            {
                do
                {
                    Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(TABLE_NAME, offset, fields, filterByFormula, maxRecords, pageSize, sort, view,
                        cellFormat, timeZone, userLocale, returnFieldsByFieldId, includeCommentCount);
                    var response = await task;

                    if (response.Success)
                    {
                        records.AddRange(response.Records.ToList());
                        offset = response.Offset;
                  }
                    else if (response.AirtableApiError is AirtableApiException)
                    {
                        errorMessage = response.AirtableApiError.ErrorMessage;
                        if (response.AirtableApiError is AirtableInvalidRequestException)
                        {
                            errorMessage += "\\\\\\\\\\nDetailed error message: ";
                            errorMessage += response.AirtableApiError.DetailedErrorMessage;
                    }
                        break;
                    }
                    else
                    {
                        errorMessage = "Unknown error";
                        break;
                    }
              } while (offset != null);
            }

            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            return new ListAllRecordsTestResponse(string.IsNullOrEmpty(errorMessage), errorMessage, records);
        }


        private async Task<ListAllRecordsTestResponse<T>> ListAllRecords<T>(
        IEnumerable<string> fields = null, 
        //string[] fields = null,
        string filterByFormula = null,
        int? maxRecords = null,
        int? pageSize = null,
        IEnumerable<Sort> sort = null,
        string view = null)
        {
            string offset = null;
            string errorMessage = null;
            var records = new List<AirtableRecord<T>>();
            try
            {
                do
                {
                    Task<AirtableListRecordsResponse<T>> task = airtableBase.ListRecords<T>(TABLE_NAME, offset, fields, filterByFormula, maxRecords, pageSize, sort, view);
                    var response = await task;

                    if (response.Success)
                    {
                        if (response.Records == null)
                        {
                            errorMessage = "Record list is empty.";
                            break;
                        }
                        records.AddRange(response.Records.ToList());
                        offset = response.Offset;
                    }
                    else if (response.AirtableApiError is AirtableApiException)
                    {
                        errorMessage = response.AirtableApiError.ErrorMessage;
                        break;
                    }
                    else
                    {
                        errorMessage = "Unknown error";
                        break;
                    }
                } while (offset != null);
            }

            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            return new ListAllRecordsTestResponse<T>(string.IsNullOrEmpty(errorMessage), errorMessage, records);
        }


        private void BuildRecordListWith3RecordsForTest(AirtableRecord[] records)
        {
            Assert.IsTrue(records != null && records.Length == 3);

            AirtableRecord record = records[0];
            record.Fields["Name"] = "Willie";
            record.Fields["Bio"] = "Bio for Willie";
            record.Fields["Bank Name"] = "Key";

            record = records[1];
            record.Fields["Name"] = "Cassie";
            record.Fields["Bio"] = "Bio for Cassie";
            record.Fields["Bank Name"] = "Citi";

            record = records[2];
            record.Fields["Name"] = "Ruby";
            record.Fields["Bio"] = "Bio for Ruby";
            record.Fields["Bank Name"] = "BOA";
        }


        private async Task<AirtableRecord[]> GetRecordsWithFormula()
        {
            string formula = "NOT({Bank Name} = '')";
            Task<ListAllRecordsTestResponse> task = ListAllRecords(filterByFormula: formula);
            var response = await task;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Records.Count > 0);
            foreach (var record in response.Records)
            {
                Assert.IsNotNull(record.GetField("Bank Name"));
            }
            return response.Records.ToArray();
        }


        private async Task<Webhooks> ListWebhooks()
        {
            Task<AirtableListWebhooksResponse> listTask = airtableBase.ListWebhooks();
            var listResponse = await listTask;
            Assert.IsTrue(listResponse.Success);
            return (listResponse.Webhooks);
        }


        private WebhooksSpecification CreateSpecForWebhook(string dataTypes)
        {
            WebhooksSpecification spec = new WebhooksSpecification();
            WebhooksOptions options = new WebhooksOptions();
            spec.Options = options;
            WebhooksFilters filters = new WebhooksFilters();
            options.Filters = filters;
            filters.RecordChangeScope = TABLE_ID;
            filters.DataTypes = new string[] { "tableData" /* , "tableFields"*/ };
            WebhooksIncludes includes = new WebhooksIncludes();
            includes.IncludePreviousCellValues = true;
            //includes.IncludeCellValuesInFieldIds = new string[] { "all" };
            options.Includes = includes;
            return spec;
        }


        private void PayloadsAnalyze(WebhooksPayload[] payloads)
        {
            foreach (var pl in payloads)
            {
                Console.WriteLine("Timestamp = {0}, BaseTransactionNumber = {1}, PayloadFormat= {2} ", pl.Timestamp, pl.BaseTransactionNumber, pl.PayloadFormat);
                PrintObject(pl.ActionMetadata);

                if (pl.ChangedTablesById != null)
                {
                    PrintChangedTable(pl.ChangedTablesById);
                }

                if (pl.CreatedTablesById != null)
                {
                }

                if (pl.DestroyedTableIds != null)
                {
                }
            }
        }


#if false
        private void PrintDestroyedTable(string[] tables)
        {
            Console.WriteLine("Number of tables destroyed: {0}", tables.Length);
            foreach (var tblName in tables)
            {
                Console.WriteLine(tblName);
            }
        }
#endif

        private void PrintObject(Object obj)
        {
            if (obj != null)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
                {
                    string name = descriptor.Name;
                    object value = descriptor.GetValue(obj);
                    Console.WriteLine("{0}={1}", name, value);
                }
            }
        }


        // There can be more than one table being changed but we will only look at the firest changed table.        
        private void PrintChangedTable(Dictionary<string, WebhooksTableChanged> chgTable)
        {
            var first = chgTable.First();
            if (first.Equals(default(KeyValuePair<string, WebhooksTableChanged>)) == false)    // first WebhooksTableChanged is not a default entry
            {
                Console.WriteLine("Key = {0}, Value = {1}", first.Key, first.Value);            // The key is the table ID. The value is the WebhooksTableChangd content that we care.

                var createdFieldsById = first.Value.CreatedFieldsById;
                if (createdFieldsById != null)                          // Fields created?
                {
                    Console.WriteLine();
                    Console.WriteLine("Fields are created in Record.");
                    var fieldItem = createdFieldsById.First();
                    if (fieldItem.Equals(default(KeyValuePair<string, WebhooksField>)) == false)
                    {
                        foreach (var kvp in createdFieldsById)
                        {
                            Console.WriteLine("Field.Id = {0}, Field.Name = {1}, Field.Type = {2}", fieldItem.Key, fieldItem.Value.Name, fieldItem.Value.Type);
                        }
                    }
                }

                var changedFieldsById = first.Value.ChangedFieldsById;
                if (changedFieldsById != null)                          // Fields changed?
                {
                    var fieldChange = changedFieldsById.First();
                    if (fieldChange.Equals(default(KeyValuePair<string, WebhooksFieldChange>)) == false)    // Dictionary not empty?
                    {
                        Console.WriteLine();
                        Console.WriteLine("Fields are changed in Record.");
                        foreach (var kvp in changedFieldsById)
                        {
                            Console.WriteLine("FieldChange.Current.FieldId = {0}, FieldChange.Current.Name", kvp.Key, kvp.Value);
                            Console.WriteLine("FieldChange.Previous.FieldId = {0}, FieldChange.Previous.Name", kvp.Key, kvp.Value);
                        }
                    }
                }

                var destroyedFieldIds = first.Value.DestroyedFieldIds;
                if (destroyedFieldIds != null)                          // Fields destroyed?
                {
                    Console.WriteLine();
                    Console.WriteLine("Fields are destroyed in Record.");
                    foreach (string fieldId in destroyedFieldIds)
                    {
                        Console.WriteLine("Destroyed Field ID = {0}", fieldId);
                    }
                }

                PrintChangedView(first.Value.CreatedRecordsById, first.Value.ChangedRecordsById, first.Value.DestroyedFieldIds);

                var changedViewsById = first.Value.ChangedViewsById;
                if (changedViewsById != null)                         // Changed Views?
                {
                    Console.WriteLine();
                    Console.WriteLine("Views are changed in table.");
                    var chgViews = changedViewsById.First();            // We only want to look at the first ChangedViews
                    if (chgViews.Equals(default(KeyValuePair<string, WebhooksChangedView>)) == false)
                    {
                        Console.WriteLine("Key = {0}, Value = {1}", chgViews.Key, chgViews.Value);   // The key is the View ID. The value is the ChangedView content that we care.
                        PrintChangedView(chgViews.Value.CreatedRecordsById, chgViews.Value.ChangedRecordsById, chgViews.Value.DestroyedRecordIds);
                    }
                }

            }
        }


        private void PrintChangedView(
            Dictionary<string, WebhooksCreatedRecord> createdRecordsById,
            Dictionary<string, WebhooksChangedRecord> changedRecordsById,
            string[] destroyedRecordIds)
        {
            if (createdRecordsById != null)                         // Reccords created?
            {
                Console.WriteLine();
                Console.WriteLine("Records are created in table.");
                var createdRcd = createdRecordsById.First();
                if (createdRcd.Equals(default(KeyValuePair<string, WebhooksCreatedRecord>)) == false)
                {
                    Console.WriteLine("Record ID = {0}, Created Time = {1}, ", createdRcd.Key, createdRcd.Value.CreatedTime);
                    var cellValue = createdRcd.Value.CellValuesByFieldId.First();
                    if (cellValue.Equals(default(KeyValuePair<string, object>)) == false)
                    {
                        foreach (var kvp in createdRcd.Value.CellValuesByFieldId)
                        {
                            Console.WriteLine("Field ID = {0}, Field Value = {1}", kvp.Key, kvp.Value);
                        }
                    }
                }
            }

            if (changedRecordsById != null)                         // Records changed
            {
                Console.WriteLine();
                Console.WriteLine("Records are changed in table.");
                var chgRecord = changedRecordsById.First();         // Look at the first chagned record.
                if (chgRecord.Equals(default(KeyValuePair<string, WebhooksChangedRecord>)) == false)
                {
                    Console.WriteLine("Key = {0}, Value = {1}", chgRecord.Key, chgRecord.Value);

                    var rcdData = chgRecord.Value.Current.CellValuesByFieldId.First();
                    Console.WriteLine("Key = {0}, Value = {1}", rcdData.Key, rcdData.Value);

                    if (chgRecord.Value.Previous != null && chgRecord.Value.Previous.CellValuesByFieldId != null)
                    {
                        rcdData = chgRecord.Value.Previous.CellValuesByFieldId.First();
                        if (rcdData.Equals(default(KeyValuePair<string, object>)) == false)
                        {
                            Console.WriteLine("Key = {0}, Value = {1}", rcdData.Key, rcdData.Value);
                        }
                    }

                    if (chgRecord.Value.Unchanged != null && chgRecord.Value.Unchanged.CellValuesByFieldId != null)
                    {
                        var unchg = chgRecord.Value.Unchanged.CellValuesByFieldId.First();
                        if (unchg.Equals(default(KeyValuePair<string, object>)) == false)
                        {
                            Console.WriteLine("Key = {0}, Value = {1}", unchg.Key, unchg.Value);
                        }
                    }
                }
            }

            if (destroyedRecordIds != null)                 // Records destroyed?
            {
                Console.WriteLine();
                Console.WriteLine("Records are destroyed in table.");
                foreach (string rcd in destroyedRecordIds)
                {
                    Console.WriteLine("Destroyed Record ID = {0}", rcd);
                }
            }
        }
    }
}
