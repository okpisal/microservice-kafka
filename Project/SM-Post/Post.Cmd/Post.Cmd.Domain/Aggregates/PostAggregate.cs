using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate :AggregateRoot
    {
        private bool _active;

        private string _author;

        private readonly Dictionary<Guid,Tuple<string,string>> _comments=new();

        public bool Active{
            get => _active;set => _active=value;
        }

        public PostAggregate()
        {
            
        }

        public PostAggregate(Guid id, string author,string message)
        {
            RaiseEvent(new PostCreateEvent(){
                Id=id,
                Author=author,
                Message=message,
                DatePosted=DateTime.Now
            });
        }

        public void Apply(PostCreateEvent @event){
            _id=@event.Id;
            _active=true;
            _author=@event.Author;
        }

        public void EditMessage(string message){

            if(!_active){
                throw new InvalidOperationException($"You Can NOt Edit the message of an inactive post !");
            }

            if(string.IsNullOrWhiteSpace(message)){
                throw new InvalidOperationException($"The Value of {nameof(message)} can not be null or empty. Please provide a valid {nameof(message)}");
            }
            RaiseEvent(new MessageUpdateEvent{
                 Id=_id,
                 Message=message
            });
        }

        public void Apply(MessageUpdateEvent @event){
            _id=@event.Id;
        }

        public void LikeMessage(){

            if(!_active){
                throw new InvalidOperationException($"You Can Not like an inactive post !");
            }

            RaiseEvent(new PostLikeEvent{
                 Id=_id,
                 
            });
        }

        public void Apply(PostLikeEvent @event){
            _id=@event.Id;
        }

         public void AddMessage(string comment,string userName){

            if(!_active){
                throw new InvalidOperationException($"You Can NOt add the comment of an inactive post !");
            }

            if(string.IsNullOrWhiteSpace(comment)){
                throw new InvalidOperationException($"The Value of {nameof(comment)} can not be null or empty. Please provide a valid {nameof(comment)}");
            }
            RaiseEvent(new CommentAddedEvent{
                 Id=_id,
                CommentId=Guid.NewGuid(),
                Comment=comment,
                UserName=userName,
                CommentDate=DateTime.Now
            });
        }

        public void Apply(CommentAddedEvent @event){
            _id=@event.Id;
            _comments.Add(@event.CommentId,new Tuple<string, string>(@event.Comment,@event.UserName));
        }

        public void EditComment(Guid commentId,string comment,string userName){
            if(!_active){
                throw new InvalidOperationException($"You Can NOt edit the comment of an inactive post !");
            }

            if(!_comments[commentId].Item2.Equals(userName,StringComparison.CurrentCultureIgnoreCase)){
                throw new InvalidOperationException($"You are not allowed to edit a comment that was made by another user !");
            }
            RaiseEvent(new CommentUpdatedEvent{
                 Id=_id,
                CommentId=commentId,
                Comment=comment,
                UserName=userName,
                EditDate=DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event){
            _id=@event.Id;
            _comments[@event.CommentId]=new Tuple<string, string>(@event.Comment,@event.UserName);

        }

        public void RemoveComment(Guid commentId,string userName){
            if(!_active){
                throw new InvalidOperationException($"You Can NOt Remove the comment of an inactive post !");
            }

            if(!_comments[commentId].Item2.Equals(userName,StringComparison.CurrentCultureIgnoreCase)){
                throw new InvalidOperationException($"You are not allowed to removed a comment that was made by another user !");
            }
            RaiseEvent(new CommentRemovedEvent{
                 Id=_id,
                CommentId=commentId,
                
            });
        }

        public void Apply(CommentRemovedEvent @event){
            _id=@event.Id;
            _comments.Remove(@event.CommentId);

        }

        public void DeletePost(string userName){
            if(!_active){
                throw new InvalidOperationException($"The Post has already been removed !");
            }

            if(!_author.Equals(userName,StringComparison.CurrentCultureIgnoreCase)){
                throw new InvalidOperationException($"You are not allowed to delete a post that was made by another user !");
            }
            RaiseEvent(new PostRemovedEvent{
                 Id=_id,
                
                
            });
        }
         public void Apply(PostRemovedEvent @event){
            _id=@event.Id;
            _active=false;

        }
        
    }
}