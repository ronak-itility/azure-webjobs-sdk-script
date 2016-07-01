﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using WebJobs.Script.ConsoleHost.Arm.Models;
using WebJobs.Script.ConsoleHost.Arm.Extensions;

namespace WebJobs.Script.ConsoleHost.Arm
{
    public partial class ArmManager
    {
        public async Task<IEnumerable<Subscription>> GetSubscriptions()
        {
            var subscriptionsResponse = await _client.HttpInvoke(HttpMethod.Get, ArmUriTemplates.Subscriptions.Bind(string.Empty));
            await subscriptionsResponse.EnsureSuccessStatusCodeWithFullError();

            var subscriptions = await subscriptionsResponse.Content.ReadAsAsync<ArmSubscriptionsArray>();
            return subscriptions.value.Select(s => new Subscription(s.subscriptionId, s.displayName));
        }

        public async Task<Subscription> Load(Subscription subscription)
        {
                var armResourceGroupsResponse = await _client.HttpInvoke(HttpMethod.Get, ArmUriTemplates.ResourceGroups.Bind(subscription));
                await armResourceGroupsResponse.EnsureSuccessStatusCodeWithFullError();

                var armResourceGroups = await armResourceGroupsResponse.Content.ReadAsAsync<ArmArrayWrapper<ArmResourceGroup>>();

                subscription.ResourceGroups = armResourceGroups.value
                    .Select(rg => new ResourceGroup(subscription.SubscriptionId, rg.name, rg.location) { Tags = rg.tags });

                return subscription;
        }
    }
}