﻿/* 
  (c) Copyright 2016 Hewlett Packard Enterprise Development LP

  Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.

  You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,

  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

  See the License for the specific language governing permissions and limitations under the License.
*/

using OctaneMyItemsSyncService.Models;
using System.Threading.Tasks;

namespace OctaneMyItemsSyncService.Services
{
  public interface IOctaneTestService
  {
    Task<Test> GetTest(string id, bool byCurrentOwner);
    Task<Tests> GetTests(string parameters = null, bool indetail = false);
    Task<Tests> GetMyTests();
    Task<Test> GetMyTest(string id);
    Task<Comments> GetTestComments(string id);
    Task<TestScript> GetTestScript(string test_id);
  }
}