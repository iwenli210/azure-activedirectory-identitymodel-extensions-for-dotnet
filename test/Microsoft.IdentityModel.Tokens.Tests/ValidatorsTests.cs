//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tests;
using Xunit;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Microsoft.IdentityModel.Tokens.Tests
{
    public class ValidatorsTests
    {
        [Theory, MemberData(nameof(AudienceDataSet))]
        public void Audience(List<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters, ExpectedException ee)
        {
            try
            {
                Validators.ValidateAudience(audiences, securityToken, validationParameters);
                ee.ProcessNoException();
            }
            catch (Exception ex)
            {
                ee.ProcessException(ex);
            }
        }

        public static TheoryData<List<string>, SecurityToken, TokenValidationParameters, ExpectedException> AudienceDataSet
        {
            get
            {
                List<string> audiences = new List<string> { "", Default.Audience };
                List<string> invalidAudiences = new List<string> { "", NotDefault.Audience };
                Dictionary<string, object> properties = new Dictionary<string, object> { { "InvalidAudience", TestUtilities.SerializeAsSingleCommaDelimitedString(audiences) } };

                var dataset = new TheoryData<List<string>, SecurityToken, TokenValidationParameters, ExpectedException>();

                dataset.Add(null, null, null, ExpectedException.ArgumentNullException());
                dataset.Add(null, null, new TokenValidationParameters { ValidateAudience = false }, ExpectedException.NoExceptionExpected);
                dataset.Add(null, null, new TokenValidationParameters(), ExpectedException.SecurityTokenInvalidAudienceException("IDX10207:"));
                dataset.Add(audiences, null, new TokenValidationParameters(), ExpectedException.SecurityTokenInvalidAudienceException("IDX10208:", propertiesExpected: properties));
                dataset.Add(audiences, null, new TokenValidationParameters { ValidAudience = NotDefault.Audience }, ExpectedException.SecurityTokenInvalidAudienceException("IDX10214:", propertiesExpected: properties));
                dataset.Add(audiences, null, new TokenValidationParameters { ValidAudiences = invalidAudiences }, ExpectedException.SecurityTokenInvalidAudienceException("IDX10214:", propertiesExpected: properties));
                dataset.Add(audiences, null, new TokenValidationParameters { ValidAudience = Default.Audience }, ExpectedException.NoExceptionExpected);
                dataset.Add(audiences, null, new TokenValidationParameters { ValidAudiences = audiences }, ExpectedException.NoExceptionExpected);

                return dataset;
            }
        }

        [Theory, MemberData(nameof(IssuerDataSet))]
        public void Issuer(string issuer, SecurityToken securityToken, TokenValidationParameters validationParameters, ExpectedException ee)
        {
            try
            {
                Validators.ValidateIssuer(issuer, securityToken, validationParameters);
                ee.ProcessNoException();
            }
            catch (Exception ex)
            {
                ee.ProcessException(ex);
            }
        }

        public static TheoryData<string, SecurityToken, TokenValidationParameters, ExpectedException> IssuerDataSet
        {
            get
            {
                List<string> issuers = new List<string> { null, "", Default.Issuer };
                List<string> invalidIssuers = new List<string> { "", NotDefault.Issuer };
                Dictionary<string, object> properties = new Dictionary<string, object> { { "InvalidIssuer", Default.Issuer } };

                var dataset = new TheoryData<string, SecurityToken, TokenValidationParameters, ExpectedException>();

                dataset.Add(null, null, null, ExpectedException.ArgumentNullException());
                dataset.Add(null, null, new TokenValidationParameters { ValidateIssuer = false }, ExpectedException.NoExceptionExpected);
                dataset.Add(null, null, new TokenValidationParameters(), ExpectedException.SecurityTokenInvalidIssuerException("IDX10211:", propertiesExpected: new Dictionary<string, object> { { "InvalidIssuer", null } }));
                dataset.Add(Default.Issuer, null, new TokenValidationParameters(), ExpectedException.SecurityTokenInvalidIssuerException("IDX10204:", propertiesExpected: properties));
                dataset.Add(Default.Issuer, null, new TokenValidationParameters { ValidIssuer = NotDefault.Issuer }, ExpectedException.SecurityTokenInvalidIssuerException("IDX10205:", propertiesExpected: properties));
                dataset.Add(Default.Issuer, null, new TokenValidationParameters { ValidIssuers = invalidIssuers }, ExpectedException.SecurityTokenInvalidIssuerException("IDX10205:", propertiesExpected: properties));
                dataset.Add(Default.Issuer, null, new TokenValidationParameters { ValidIssuer = Default.Issuer }, ExpectedException.NoExceptionExpected);
                dataset.Add(Default.Issuer, null, new TokenValidationParameters { ValidIssuers = issuers }, ExpectedException.NoExceptionExpected);

                return dataset;
            }
        }

        [Theory, MemberData(nameof(LifeTimeDataSet))]
        public void Lifetime(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters, ExpectedException ee)
        {
            try
            {
                Validators.ValidateLifetime(notBefore, expires, securityToken, validationParameters);
                ee.ProcessNoException();
            }
            catch (Exception ex)
            {
                ee.ProcessException(ex);
            }
        }

        public static TheoryData<DateTime?, DateTime?, SecurityToken, TokenValidationParameters, ExpectedException> LifeTimeDataSet
        {
            get
            {
                List<string> issuers = new List<string> { "", Default.Issuer };
                List<string> invalidIssuers = new List<string> { "", NotDefault.Issuer };
                DateTime? notBefore;
                DateTime? expires;

                //                           notbefore  expires    
                var dataset = new TheoryData<DateTime?, DateTime?, SecurityToken, TokenValidationParameters, ExpectedException>();

                dataset.Add(null, null, null, null, ExpectedException.ArgumentNullException());
                dataset.Add(null, null, null, new TokenValidationParameters { ValidateLifetime = false }, ExpectedException.NoExceptionExpected);
                dataset.Add(null, null, null, new TokenValidationParameters(), ExpectedException.SecurityTokenNoExpirationException("IDX10225:"));

                notBefore = EpochTime.DateTime(EpochTime.GetIntDate((DateTime.UtcNow + TimeSpan.FromHours(1)).ToUniversalTime()));
                expires = EpochTime.DateTime(EpochTime.GetIntDate(DateTime.UtcNow.ToUniversalTime()));
                dataset.Add(notBefore, expires, null, new TokenValidationParameters(), ExpectedException.SecurityTokenInvalidLifetimeException("IDX10224:", propertiesExpected: new Dictionary<string, object> { { "NotBefore", notBefore }, { "Expires", expires } }));

                notBefore = EpochTime.DateTime(EpochTime.GetIntDate((DateTime.UtcNow + TimeSpan.FromHours(1)).ToUniversalTime()));
                expires = EpochTime.DateTime(EpochTime.GetIntDate((DateTime.UtcNow + TimeSpan.FromHours(2)).ToUniversalTime()));
                dataset.Add(notBefore, expires, null, new TokenValidationParameters(), ExpectedException.SecurityTokenNotYetValidException("IDX10222:", propertiesExpected: new Dictionary<string, object> { { "NotBefore", notBefore } }));

                dataset.Add(DateTime.UtcNow - TimeSpan.FromHours(2), DateTime.UtcNow - TimeSpan.FromHours(1), null, new TokenValidationParameters(), ExpectedException.SecurityTokenExpiredException("IDX10223:"));
                dataset.Add(DateTime.UtcNow - TimeSpan.FromHours(2), DateTime.UtcNow + TimeSpan.FromHours(1), null, new TokenValidationParameters(), ExpectedException.NoExceptionExpected);

                // clock skew, positive then negative
                dataset.Add(DateTime.UtcNow + TimeSpan.FromMinutes(2), DateTime.UtcNow + TimeSpan.FromHours(1), null, new TokenValidationParameters{ ClockSkew = TimeSpan.FromMinutes(5) }, ExpectedException.NoExceptionExpected);
                dataset.Add(DateTime.UtcNow - TimeSpan.FromMinutes(2), DateTime.UtcNow - TimeSpan.FromMinutes(1), null, new TokenValidationParameters{ ClockSkew = TimeSpan.FromMinutes(5) }, ExpectedException.NoExceptionExpected);

                notBefore = EpochTime.DateTime(EpochTime.GetIntDate((DateTime.UtcNow + TimeSpan.FromMinutes(6)).ToUniversalTime()));
                expires = EpochTime.DateTime(EpochTime.GetIntDate((DateTime.UtcNow + TimeSpan.FromHours(1)).ToUniversalTime()));
                dataset.Add(notBefore, expires, null, new TokenValidationParameters{ ClockSkew = TimeSpan.FromMinutes(5) }, ExpectedException.SecurityTokenNotYetValidException("IDX10222:", propertiesExpected: new Dictionary<string, object> { { "NotBefore", notBefore } }));

                dataset.Add(DateTime.UtcNow - TimeSpan.FromHours(2), DateTime.UtcNow - TimeSpan.FromMinutes(6), null, new TokenValidationParameters{ ClockSkew = TimeSpan.FromMinutes(5) }, ExpectedException.SecurityTokenExpiredException("IDX10223:"));

                return dataset;
            }
        }

        [Theory, MemberData(nameof(SecurityKeyDataSet))]
        public void SecurityKey(SecurityKey securityKey, SecurityToken securityToken, TokenValidationParameters validationParameters, ExpectedException ee)
        {
            try
            {
                Validators.ValidateIssuerSecurityKey(securityKey, securityToken, validationParameters);
                ee.ProcessNoException();
            }
            catch (Exception ex)
            {
                ee.ProcessException(ex);
            }
        }

        public static TheoryData<SecurityKey, SecurityToken, TokenValidationParameters, ExpectedException> SecurityKeyDataSet
        {
            get
            {
                var dataset = new TheoryData<SecurityKey, SecurityToken, TokenValidationParameters, ExpectedException>();

                dataset.Add(null, new JwtSecurityToken(), new TokenValidationParameters { ValidateIssuerSigningKey = true }, ExpectedException.ArgumentNullException());
                dataset.Add(KeyingMaterial.SymmetricSecurityKey2_256, null, new TokenValidationParameters { ValidateIssuerSigningKey = true }, ExpectedException.ArgumentNullException());
                dataset.Add(KeyingMaterial.SymmetricSecurityKey2_256, new JwtSecurityToken(), null, ExpectedException.ArgumentNullException());
                dataset.Add(KeyingMaterial.SymmetricSecurityKey2_256, new JwtSecurityToken(), new TokenValidationParameters { ValidateIssuerSigningKey = true }, ExpectedException.NoExceptionExpected);
                dataset.Add(null, new JwtSecurityToken(), new TokenValidationParameters { ValidateIssuerSigningKey = false }, ExpectedException.NoExceptionExpected);
                dataset.Add(KeyingMaterial.SymmetricSecurityKey2_256, null, new TokenValidationParameters { ValidateIssuerSigningKey = false }, ExpectedException.NoExceptionExpected);
                dataset.Add(KeyingMaterial.ExpiredX509SecurityKey_Public, new JwtSecurityToken(), new TokenValidationParameters { ValidateIssuerSigningKey = true }, ExpectedException.SecurityTokenInvalidSigningKeyException(substringExpected: "IDX10249:"));
                dataset.Add(KeyingMaterial.NotYetValidX509SecurityKey_Public, new JwtSecurityToken(), new TokenValidationParameters { ValidateIssuerSigningKey = true }, ExpectedException.SecurityTokenInvalidSigningKeyException(substringExpected: "IDX10248:"));
                return dataset;
            }
        }

        [Theory, MemberData(nameof(TokenReplayDataSet))]
        public void TokenReplay(string securityToken, DateTime? expirationTime, TokenValidationParameters validationParameters, ExpectedException ee)
        {
            try
            {
                Validators.ValidateTokenReplay(expirationTime, securityToken, validationParameters);
                ee.ProcessNoException();
            }
            catch (Exception ex)
            {
                ee.ProcessException(ex);
            }
        }

        public static TheoryData<string, DateTime?, TokenValidationParameters, ExpectedException> TokenReplayDataSet
        {
            get
            {
                var dataset = new TheoryData<string, DateTime?, TokenValidationParameters, ExpectedException>();

                dataset.Add(null, null, new TokenValidationParameters { ValidateTokenReplay = true }, ExpectedException.ArgumentNullException());
                dataset.Add(string.Empty, null, new TokenValidationParameters { ValidateTokenReplay = true }, ExpectedException.ArgumentNullException());
                dataset.Add("token", DateTime.UtcNow + TimeSpan.FromDays(1), null, ExpectedException.ArgumentNullException());
                dataset.Add("token", null, new TokenValidationParameters { ValidateTokenReplay = true, TokenReplayCache = new TokenReplayCache { AddRetVal = true, FindRetVal = true } }, ExpectedException.SecurityTokenNoExpirationException());
                dataset.Add("token", DateTime.UtcNow + TimeSpan.FromDays(1), new TokenValidationParameters { ValidateTokenReplay = true, TokenReplayCache = new TokenReplayCache { AddRetVal = true, FindRetVal = true } }, ExpectedException.SecurityTokenReplayDetected("IDX10228:"));
                dataset.Add("token", DateTime.UtcNow + TimeSpan.FromDays(1), new TokenValidationParameters { ValidateTokenReplay = true, TokenReplayCache = new TokenReplayCache { AddRetVal = false, FindRetVal = false } }, ExpectedException.SecurityTokenReplayAddFailed("IDX10229:"));
                dataset.Add("token", DateTime.UtcNow + TimeSpan.FromDays(1), new TokenValidationParameters { ValidateTokenReplay = true, TokenReplayCache = new TokenReplayCache { AddRetVal = true, FindRetVal = false } }, ExpectedException.NoExceptionExpected);

                return dataset;
            }
        }

        // Each TokenReplayValidator in this test checks that the expiration parameter passed into it is equal to the expiration time of the token.
        // If they're not equal, the test will fail.
        [Theory, MemberData(nameof(CheckParametersForTokenReplayTheoryData))]
        public void CheckParametersForTokenReplay(TokenReplayTheoryData theoryData)
        {
            TestUtilities.WriteHeader($"{this}.CheckParametersForTokenReplay", theoryData);
            var context = new CompareContext($"{this}.CheckParametersForTokenReplay, {theoryData.TestId}");
            var tvp = new TokenValidationParameters();
            tvp.IssuerSigningKey = theoryData.SigningKey;
            tvp.TokenReplayValidator = theoryData.TokenReplayValidator;
            tvp.ValidateTokenReplay = theoryData.ValidateTokenReplay;
            tvp.ValidateAudience = false;
            tvp.ValidateIssuer = false;
            tvp.ValidateLifetime = false;
            var token = theoryData.SecurityToken;
            var tokenValidator = theoryData.SecurityTokenHandler;

            try
            {
                // TokenReplayValidator should always be provided for these tests.
                tokenValidator.ValidateToken(token, tvp, out SecurityToken validatedToken);
                theoryData.ExpectedException.ProcessNoException(context.Diffs);                
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context.Diffs);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<TokenReplayTheoryData> CheckParametersForTokenReplayTheoryData
        {
            get
            {
                return TestTheoryData.CheckParametersForTokenReplayTheoryData;
            }
        }

        class TokenReplayCache : ITokenReplayCache
        {
            public bool AddRetVal { get; set; }

            public bool FindRetVal { get; set; }

            public bool TryAdd(string securityToken, DateTime expiresOn)
            {
                return AddRetVal;
            }

            public bool TryFind(string securityToken)
            {
                return FindRetVal;
            }
        }
    }
}

#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
