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
using System.Threading.Tasks;
using Crypter.Common.Client.Interfaces.HttpClients;
using Crypter.Common.Client.Interfaces.Services;
using Crypter.Common.Client.Interfaces.Services.UserSettings;
using Crypter.Common.Contracts.Features.UserSettings.ProfileSettings;
using EasyMonads;

namespace Crypter.Common.Client.Services.UserSettings
{
   public class UserProfileSettingsService : IUserProfileSettingsService, IDisposable
   {
      private readonly ICrypterApiClient _crypterApiClient;
      private readonly IUserSessionService _userSessionService;

      private Maybe<ProfileSettings> _profileSettings;

      public UserProfileSettingsService(ICrypterApiClient crypterApiClient, IUserSessionService userSessionService)
      {
         _crypterApiClient = crypterApiClient;
         _userSessionService = userSessionService;

         _userSessionService.UserLoggedOutEventHandler += Recycle;
      }

      public async Task<Maybe<ProfileSettings>> GetProfileSettingsAsync()
      {
         if (_profileSettings.IsNone)
         {
            _profileSettings = await _crypterApiClient.UserSetting.GetProfileSettingsAsync();
         }

         return _profileSettings;
      }

      public async Task<Either<SetProfileSettingsError, ProfileSettings>> SetProfileSettingsAsync(ProfileSettings newProfileSettings)
      {
         Either<SetProfileSettingsError, ProfileSettings> result = await _crypterApiClient.UserSetting.SetProfileSettingsAsync(newProfileSettings);
         _profileSettings = result.ToMaybe();
         return result;
      }

      private void Recycle(object sender, EventArgs _)
      {
         _profileSettings = Maybe<ProfileSettings>.None;
      }

      public void Dispose()
      {
         _userSessionService.UserLoggedOutEventHandler -= Recycle;
         GC.SuppressFinalize(this);
      }
   }
}
