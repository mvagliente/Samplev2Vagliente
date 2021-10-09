<template>
  <div v-if="users != null && users.length == 0">
    <input
      type="text"
      placeholder="Search office by address"
      v-model="searchPattern"
    />
    <button @click="onOfficeSearch">Search</button>
    <button @click="onUserSearch">Get Users</button>
    <div class="user-list">
      <div v-for="office in offices" v-bind:key="office.id">
        <div>
          <input
            type="checkbox"
            :checked="selectedOffices.indexOf(office.id) >= 0"
            @click="onSelectOffice(office.id)"
          />
          Address: <span class="bold"> {{ office.address }} </span>
        </div>
      </div>
    </div>
  </div>
  <div v-if="users.length > 0">
    <UserList :users="users" />
  </div>
</template>

<script lang="ts">
import { ref } from "vue";
import UserList from "./components/UserList.vue";
import officeService, { Office } from "./services/OfficeService";
import userService, { User } from "./services/UserService";

export default {
  components: { UserList },
  setup() {
    let searchPattern = ref("");
    let offices = ref([] as Office[]);
    let selectedOffices = ref([] as string[]);
    let users = ref([] as User[]);

    function onOfficeSearch() {
      officeService
        .getOffices(searchPattern.value)
        ?.then((o) => (offices.value = o));
    }

    function onUserSearch() {
        users.value = userService.getUsers(selectedOffices.value);
    }

    function onSelectOffice(id: string) {
      if (selectedOffices.value.indexOf(id) >= 0) {
        selectedOffices.value = selectedOffices.value.filter((o) => o != id);
      } else {
        selectedOffices.value.push(id);
      }
    }

    return {
      searchPattern,
      offices,
      users,
      selectedOffices,
      onOfficeSearch,
      onUserSearch,
      onSelectOffice,
    };
  },
};
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
.bold {
  font-weight: bold;
}
.user-list {
  text-align: left;
}
</style>
