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

using System.Threading.Tasks;
using Crypter.Common.Client.Interfaces.Services;
using Microsoft.AspNetCore.Components;

namespace Crypter.Web.Pages
{
   public partial class ForgotPassword : ComponentBase
   {
      [Inject]
      protected IUserRecoveryService _userRecoveryService { get; init; }

      protected string EmailAddress { get; set; } = string.Empty;
      protected string ErrorMessage { get; set; } = string.Empty;
      protected string EmailAddressInvalidClassPlaceholder { get; set; } = string.Empty;
      protected bool Success { get; set; } = false;

      protected async Task SubmitAsync()
      {
         if (Common.Primitives.EmailAddress.TryFrom(EmailAddress, out Common.Primitives.EmailAddress validEmailAddress))
         {
            EmailAddressInvalidClassPlaceholder = string.Empty;
            ErrorMessage = string.Empty;
            await _userRecoveryService.RequestRecoveryEmailAsync(validEmailAddress);
            Success = true;
         }
         else
         {
            EmailAddressInvalidClassPlaceholder = "is-invalid";
            ErrorMessage = "Invalid email address";
         }
      }
   }
}
