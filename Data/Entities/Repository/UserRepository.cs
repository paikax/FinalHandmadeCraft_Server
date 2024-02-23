// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Data.Entities.BaseRepository;
// using Data.Entities.User;
//
// namespace Data.Entities.Repository
// {
//     public static class UserRepository
//     {
//         public static Task UpdateRefreshToken(this IBaseRepository<User.User> repository, string id, List<RefreshToken> refreshTokens,  string updateBy)
//         {
//             var updates = new List<UpdateManyEntitiesParams<User.User, dynamic>>
//             {
//                 new UpdateManyEntitiesParams<User.User, dynamic> {Field = _ => _.UpdateAt, Value = DateTime.UtcNow},
//                 new UpdateManyEntitiesParams<User.User, dynamic> {Field = _ => _.RefreshTokens, Value = refreshTokens}
//             };
//
//             return repository.UpdateOneAsync(_ => _.Id == id && !_.IsDeleted, updates);
//         }
//     }
// }