using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Litium.Accelerator.Constants;
using Litium.Account;
using Litium.Events;
using Litium.Runtime.DependencyInjection;
using Litium.Security;
using Litium.Security.Events;

namespace Litium.Accelerator.Services
{
    [ServiceDecorator(typeof(IAccountService))]
    internal class AccountServiceDecorator : IAccountService
    {
        private readonly IAccountService _parentResolver;
        private readonly PrincipalContextService _principalContextService;
        private static readonly AsyncLocal<ISet<Guid>> _localOrganization = new();

        public AccountServiceDecorator(
            IAccountService parentResolver,
            PrincipalContextService principalContextService,
            EventBroker eventBroker)
        {
            _parentResolver = parentResolver;
            _principalContextService = principalContextService;
            eventBroker.Subscribe<PersonSignedIn>(EventScope.Context, _ => _localOrganization.Value = null);
            eventBroker.Subscribe<PersonSignedOut>(EventScope.Context, _ => _localOrganization.Value = null);
        }

        public ISet<Guid> GetGroupsSystemId(Guid personSystemId)
        {
            return _parentResolver.GetGroupsSystemId(personSystemId);
        }

        public ISet<Guid> GetOrganizationsSystemId(Guid personSystemId)
        {
            var current = _localOrganization.Value;
            if (current is not null)
            {
                return current;
            }
            else
            {
                var organizationClaim = _principalContextService.GetCurrentPrincipal()?.FindFirstValue(AcceleratorClaimTypes.SelectedOrganization);
                if (organizationClaim is not null
                    && Guid.TryParse(organizationClaim, out var organizationSystemId))
                {
                    _localOrganization.Value = current = new HashSet<Guid> { organizationSystemId };
                    return current;
                }
            }

            _localOrganization.Value = current = _parentResolver.GetOrganizationsSystemId(personSystemId);
            return current;
        }
    }
}
