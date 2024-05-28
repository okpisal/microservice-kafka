using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        private readonly List<BaseEvent> _change=new();

        public Guid Id{
            get {return _id;}
        }
        public int Version { get; set; }=-1;

        public IEnumerable<BaseEvent> GetUncommitedChanges(){
            return _change;
        }

        public void MarkChangesAsCommited(){
            _change.Clear();
        }

        private void ApplyChange(BaseEvent @event,bool isNew)
        {
            var method=this.GetType().GetMethod("Apply",new Type[]{@event.GetType()});
            if(method==null){
                throw new ArgumentNullException(nameof(method),"this apply method was not found in the aggregate for "+@event.GetType().Name+" !");
            }
            method.Invoke(this,new object[]{@event});
            if(isNew){
                _change.Add(@event);
            }
        }
        protected void RaiseEvent(BaseEvent @event){
            this.ApplyChange(@event,true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events){
            foreach (var @event in events)
            {
                this.ApplyChange(@event,false);
            }
        }
        
    }
}