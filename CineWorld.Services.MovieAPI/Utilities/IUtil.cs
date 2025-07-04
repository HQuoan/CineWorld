﻿using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.MovieAPI.Utilities
{
  public interface IUtil
  {
    List<string> GetUserRoles();
    bool IsInRoles(IEnumerable<string> rolesToCheck);
    bool IsUniqueConstraintViolation(DbUpdateException ex);
  }
}