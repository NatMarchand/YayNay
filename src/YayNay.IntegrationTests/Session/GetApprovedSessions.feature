Feature: Get approved sessions
  Scenario: Get approved sessions when no session returns empty list
    Given user has right ScheduleSession
    When GET to sessions?status=Approved
    And authenticated as a user
    Then status code is OK
    And content matches
"""
{
  "values": [],
  "paging": {
    "currentPage": 0,
    "totalPages": 1
  }
}
"""

  Scenario: Get approved sessions when some sessions returns list
    Given user has right ScheduleSession
    And a session entitled Session 1 with status Approved
    And a session entitled Session 2 with status Approved
    And a session entitled Session 3 with status Approved
    And a session entitled Session 4 with status Approved
    When GET to sessions?status=Approved
    And authenticated as a user
    Then status code is OK
    And content matches
"""
{
  "values": [
    {
      "id": "*",
      "title": "Session 1",
      "description": "",
      "schedule": null,
      "status": "Approved",
      "tags": [],
      "speakers": []
    },
    {
      "id": "*",
      "title": "Session 2",
      "description": "",
      "schedule": null,
      "status": "Approved",
      "tags": [],
      "speakers": []
    },
    {
      "id": "*",
      "title": "Session 3",
      "description": "",
      "schedule": null,
      "status": "Approved",
      "tags": [],
      "speakers": []
    },
    {
      "id": "*",
      "title": "Session 4",
      "description": "",
      "schedule": null,
      "status": "Approved",
      "tags": [],
      "speakers": []
    }
  ],
  "paging": {
    "currentPage": 0,
    "totalPages": 1
  }
}
"""

  Scenario: Get approved sessions when no right returns empty list
    When GET to sessions?status=Approved
    And authenticated as a user
    Then status code is OK
    And content matches
"""
{
  "values": [],
  "paging": null
}
"""

  Scenario: Get approved sessions when not authenticated returns empty list
    When GET to sessions?status=Approved
    Then status code is OK
    And content matches
"""
{
  "values": [],
  "paging": null
}
"""