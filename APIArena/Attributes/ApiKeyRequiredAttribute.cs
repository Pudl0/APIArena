using APIArena.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace APIArena.Attributes
{
    public class ApiKeyRequired : Attribute, IAuthorizationRequirement, IAuthorizationRequirementData
    {
        /// <summary>
        /// One of the scopes in this list must be applicable.
        /// </summary>
		public string[]? Scopes { get; set; }

        /// <summary>
        /// When <see langword="true"/> all <see cref="Scopes"/> need to apply. Otherwise only one
        /// must be present in the <see cref="ApiKey"/>.
        /// </summary>
        public bool RequiresAllScopes { get; set; } = false;

        /// <summary>
        /// When <see langword="true"/> the request needs to also pass the <see cref="DenyAnonymousAuthorizationRequirement"/>. Otherwise only
        /// a valid <see cref="ApiKey"/> with the correct scopes is required.
        /// </summary>
        public bool RequireAuthentication { get; set; } = false;

        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return this;
        }
    }
}
