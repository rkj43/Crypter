﻿/*
 * Copyright (C) 2023 Crypter File Transfer
 * 
 * This file is part of the Crypter file transfer project.
 * 
 * Crypter is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * The Crypter source code is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * You can be released from the requirements of the aforementioned license
 * by purchasing a commercial license. Buying such a license is mandatory
 * as soon as you develop commercial activities involving the Crypter source
 * code without disclosing the source code of your own applications.
 * 
 * Contact the current copyright holder to discuss commercial license options.
 */

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Crypter.Common.Contracts;
using Crypter.Common.Contracts.Features.UserSettings;
using Crypter.Common.Contracts.Features.UserSettings.ContactInfoSettings;
using Crypter.Common.Contracts.Features.UserSettings.NotificationSettings;
using Crypter.Common.Contracts.Features.UserSettings.PrivacySettings;
using Crypter.Common.Contracts.Features.UserSettings.ProfileSettings;
using Crypter.Core.Services;
using Crypter.Core.Services.UserSettings;
using EasyMonads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Crypter.API.Controllers
{
   [ApiController]
   [Route("api/user/setting")]
   public class UserSettingController : CrypterControllerBase
   {
      private readonly ITokenService _tokenService;
      private readonly IUserProfileSettingsService _userProfileSettingsService;
      private readonly IUserContactInfoSettingsService _userContactInfoSettingsService;
      private readonly IUserNotificationSettingsService _userNotificationSettingsService;
      private readonly IUserPrivacySettingsService _userPrivacySettingsService;
      private readonly IUserEmailVerificationService _userEmailVerificationService;

      public UserSettingController(ITokenService tokenService, IUserProfileSettingsService userProfileSettingsService, IUserContactInfoSettingsService userContactInfoSettingsService, IUserNotificationSettingsService userNotificationSettingsService, IUserPrivacySettingsService userPrivacySettingsService, IUserEmailVerificationService userEmailVerificationService)
      {
         _tokenService = tokenService;
         _userProfileSettingsService = userProfileSettingsService;
         _userContactInfoSettingsService = userContactInfoSettingsService;
         _userNotificationSettingsService = userNotificationSettingsService;
         _userPrivacySettingsService = userPrivacySettingsService;
         _userEmailVerificationService = userEmailVerificationService;
      }

      [HttpGet("profile")]
      [Authorize]
      [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfileSettings))]
      [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
      [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
      public async Task<IActionResult> GetProfileSettingsAsync(CancellationToken cancellationToken)
      {
         IActionResult MakeErrorResponse(GetProfileSettingsError error)
         {
#pragma warning disable CS8524
            return error switch
            {
               GetProfileSettingsError.UnknownError => MakeErrorResponseBase(HttpStatusCode.InternalServerError, error),
               GetProfileSettingsError.NotFound => MakeErrorResponseBase(HttpStatusCode.NotFound, error)
            };
#pragma warning restore CS8524
         }

         Guid userId = _tokenService.ParseUserId(User);
         return await _userProfileSettingsService.GetProfileSettingsAsync(userId, cancellationToken)
            .MatchAsync(
               () => MakeErrorResponse(GetProfileSettingsError.NotFound),
               Ok);
      }

      [HttpPut("profile")]
      [Authorize]
      [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfileSettings))]
      [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
      public async Task<IActionResult> SetProfileSettingsAsync([FromBody] ProfileSettings request)
      {
         IActionResult MakeErrorResponse(SetProfileSettingsError error)
         {
#pragma warning disable CS8524
            return error switch
            {
               SetProfileSettingsError.UnknownError => MakeErrorResponseBase(HttpStatusCode.InternalServerError, error)
            };
#pragma warning restore CS8524
         }

         Guid userId = _tokenService.ParseUserId(User);
         return await _userProfileSettingsService.SetProfileSettingsAsync(userId, request)
            .MatchAsync(
               MakeErrorResponse,
               Ok,
               MakeErrorResponse(SetProfileSettingsError.UnknownError));
      }

      [HttpGet("contact")]
      [Authorize]
      [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContactInfoSettings))]
      [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
      [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
      public async Task<IActionResult> GetContactInfoSettingsAsync(CancellationToken cancellationToken)
      {
         IActionResult MakeErrorResponse(GetContactInfoSettingsError error)
         {
#pragma warning disable CS8524
            return error switch
            {
               GetContactInfoSettingsError.UnknownError => MakeErrorResponseBase(HttpStatusCode.InternalServerError, error),
               GetContactInfoSettingsError.NotFound => MakeErrorResponseBase(HttpStatusCode.NotFound, error)
            };
#pragma warning restore CS8524
         }

         Guid userId = _tokenService.ParseUserId(User);
         return await _userContactInfoSettingsService.GetContactInfoSettingsAsync(userId, cancellationToken)
            .MatchAsync(
               () => MakeErrorResponse(GetContactInfoSettingsError.NotFound),
               Ok);
      }

      [HttpPost("contact")]
      [Authorize]
      [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContactInfoSettings))]
      [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
      [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorResponse))]
      [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
      public async Task<IActionResult> UpdateContactInfoSettingsAsync(UpdateContactInfoSettingsRequest request)
      {
         IActionResult MakeErrorResponse(UpdateContactInfoSettingsError error)
         {
#pragma warning disable CS8524
            return error switch
            {
               UpdateContactInfoSettingsError.UnknownError
                  or UpdateContactInfoSettingsError.PasswordHashFailure => MakeErrorResponseBase(HttpStatusCode.InternalServerError, error),
               UpdateContactInfoSettingsError.UserNotFound => MakeErrorResponseBase(HttpStatusCode.NotFound, error),
               UpdateContactInfoSettingsError.EmailAddressUnavailable => MakeErrorResponseBase(HttpStatusCode.Conflict, error),
               UpdateContactInfoSettingsError.InvalidEmailAddress
                  or UpdateContactInfoSettingsError.InvalidPassword
                  or UpdateContactInfoSettingsError.PasswordNeedsMigration
                  or UpdateContactInfoSettingsError.InvalidUsername => MakeErrorResponseBase(HttpStatusCode.BadRequest, error)
            };
#pragma warning restore CS8524
         }

         Guid userId = _tokenService.ParseUserId(User);
         return await _userContactInfoSettingsService.UpdateContactInfoSettingsAsync(userId, request)
            .MatchAsync(
               MakeErrorResponse,
               Ok,
               MakeErrorResponse(UpdateContactInfoSettingsError.UnknownError));
      }

      [HttpPost("contact/verify")]
      [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
      public async Task<IActionResult> VerifyUserEmailAddressAsync([FromBody] VerifyEmailAddressRequest request)
      {
         return await _userEmailVerificationService.VerifyUserEmailAddressAsync(request)
            ? Ok()
            : MakeErrorResponseBase(HttpStatusCode.NotFound, VerifyEmailAddressError.NotFound);
      }

      [HttpGet("notification")]
      [Authorize]
      [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotificationSettings))]
      [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
      [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
      public async Task<IActionResult> GetNotificationSettingsAsync(CancellationToken cancellationToken)
      {
         IActionResult MakeErrorResponse(GetNotificationSettingsError error)
         {
#pragma warning disable CS8524
            return error switch
            {
               GetNotificationSettingsError.UnknownError => MakeErrorResponseBase(HttpStatusCode.InternalServerError, error),
               GetNotificationSettingsError.NotFound => MakeErrorResponseBase(HttpStatusCode.NotFound, error)
            };
#pragma warning restore CS8524
         }

         Guid userId = _tokenService.ParseUserId(User);
         return await _userNotificationSettingsService.GetNotificationSettingsAsync(userId, cancellationToken)
            .MatchAsync(
               () => MakeErrorResponse(GetNotificationSettingsError.UnknownError),
               Ok);
      }

      [HttpPost("notification")]
      [Authorize]
      [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotificationSettings))]
      [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
      [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
      public async Task<IActionResult> UpdateNotificationSettingsAsync(NotificationSettings request)
      {
         IActionResult MakeErrorResponse(UpdateNotificationSettingsError error)
         {
#pragma warning disable CS8524
            return error switch
            {
               UpdateNotificationSettingsError.UnknownError => MakeErrorResponseBase(HttpStatusCode.InternalServerError, error),
               UpdateNotificationSettingsError.EmailAddressNotVerified => MakeErrorResponseBase(HttpStatusCode.BadRequest, error)
            };
#pragma warning restore CS8524
         }

         Guid userId = _tokenService.ParseUserId(User);
         return await _userNotificationSettingsService.UpdateNotificationSettingsAsync(userId, request)
            .MatchAsync(
               MakeErrorResponse,
               Ok,
               MakeErrorResponse(UpdateNotificationSettingsError.UnknownError));
      }

      [HttpGet("privacy")]
      [Authorize]
      [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PrivacySettings))]
      [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
      [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
      public async Task<IActionResult> GetPrivacySettingsAsync(CancellationToken cancellationToken)
      {
         IActionResult MakeErrorResponse(GetPrivacySettingsError error)
         {
#pragma warning disable CS8524
            return error switch
            {
               GetPrivacySettingsError.UnknownError => MakeErrorResponseBase(HttpStatusCode.InternalServerError, error),
               GetPrivacySettingsError.NotFound => MakeErrorResponseBase(HttpStatusCode.NotFound, error)
            };
#pragma warning restore CS8524
         }

         Guid userId = _tokenService.ParseUserId(User);
         return await _userPrivacySettingsService.GetPrivacySettingsAsync(userId, cancellationToken)
            .MatchAsync(
               () => MakeErrorResponse(GetPrivacySettingsError.NotFound),
               Ok);
      }

      [HttpPut("privacy")]
      [Authorize]
      [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PrivacySettings))]
      [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
      [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
      public async Task<IActionResult> SetPrivacySettingsAsync(PrivacySettings request)
      {
         IActionResult MakeErrorResponse(SetPrivacySettingsError error)
         {
#pragma warning disable CS8524
            return error switch
            {
               SetPrivacySettingsError.UnknownError => MakeErrorResponseBase(HttpStatusCode.InternalServerError, error)
            };
#pragma warning restore CS8524
         };

         Guid userId = _tokenService.ParseUserId(User);
         return await _userPrivacySettingsService.SetPrivacySettingsAsync(userId, request)
            .MatchAsync(
               MakeErrorResponse,
               Ok,
               MakeErrorResponse(SetPrivacySettingsError.UnknownError));
      }
   }
}
