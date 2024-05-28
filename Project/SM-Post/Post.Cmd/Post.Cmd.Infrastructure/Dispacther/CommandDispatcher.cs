using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispacther
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type,Func<BaseCommand,Task>> _handler=new();
        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if(_handler.ContainsKey(typeof(T))){
                throw new IndexOutOfRangeException("You Can't register the same command handler twice !");
            }
            _handler.Add(typeof(T),x=>handler((T)x));
        }

        public async Task SendAsync(BaseCommand baseCommand)
        {
            if(_handler.TryGetValue(baseCommand.GetType(),out Func<BaseCommand,Task> handler)){
                await handler(baseCommand);
            }else{
                throw new ArgumentNullException(nameof(handler),"No Command handler was register");
            }

        }
    }
}