<template>
  <input type="text" placeholder="Search by login" v-model="searchPattern" />
  <div class="user-list">
    <div v-for="item in data" v-bind:key="item.id">
      <div>
        Login: <span class="bold"> {{ item.login }} </span>
        <span> Roles:</span>
        <span
          class="bold"
          v-for="role in item.roles"
          v-bind:key="role.role.id"
        >
          {{ role.role.name }}
        </span>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { ref, watch } from "vue";
import type from "@/core/Type";
import userService, { User } from "@/services/UserService";

export default {
  props: {
    officeIds: type<string[]>(),
  },
  setup(props) {
    let searchPattern = ref("");
    let data = ref([] as User[]);

    watch(searchPattern, (newValue) => {
      data.value = userService.getUsers(props.officeIds)
        ?.filter((o) => o.login.indexOf(newValue) >= 0);
    });

    return {
      searchPattern,
      data,
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