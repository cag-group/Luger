using System;
using Luger.Functional;

namespace Luger.Examples.ES
{
    /* ES systems have several main components.
     * Immutable Data Objects:
     *  Commands    - Represent client request.
     *                  Ex. SetCustomerNameCommand { customerId, name }
     *  Events      - Serializable. Represent past events.
     *                  Ex. CustomerNameSetEvent : Event<CustomerId, string> { name }
     *  States      - Represent the state of an entity at a point in time.
     *                  Ex. CustomerState { Id, Name }
     *  Read Models - Represent any queryable state.
     *                  Ex. CustomersByName : Dict<string, CustomerId>
     * 
     * Functions:
     *  State transitions   - Evolves state with events. state -> event -> state
     *  Event handlers      - Subscribes to events. Perform business logic. event -> unit
     */

    public struct CustomerId
    {
        private readonly Some<string> _id;

        public CustomerId(Some<string> repr) => _id = repr;
    }

    public class SetCustomerNameCommand
    {
        public readonly CustomerId CustomerId;

        public readonly Some<string> Name;

        public SetCustomerNameCommand(CustomerId id, Some<string> name) => (CustomerId, Name)=(id, name);
    }
}
