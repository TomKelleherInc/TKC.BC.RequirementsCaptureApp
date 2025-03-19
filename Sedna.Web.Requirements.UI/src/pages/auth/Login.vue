<template>
  <div class="login">

    <card header-text="Login">
      <div class="card-body card-block">
        Use your windows login to access this service.
      </div>
      
      <div class="card-body card-block">
        <form method="post" action="/auth/login" name="login">
          <div class="form-group">
            <div class="input-group">
              <div class="input-group-addon"><i class="fa fa-user"></i></div>
              <input type="email" id="email" name="email" placeholder="Username" class="form-control">
            </div>
          </div>
          <div class="form-group">
            <div class="input-group">
              <div class="input-group-addon"><i class="fa fa-asterisk"></i></div>
              <input type="password" id="password" name="password" placeholder="Password" class="form-control">
            </div>
          </div>
          <div class="form-actions form-group">
              <button type="submit" class="btn btn-primary btn-md">Log In</button>
          </div>
        </form>
      </div>
    </card>

  </div>
</template>

<script>
export default {
  name: 'Login'
}
</script>

<style lang="scss" scoped>
  .card-title{
    padding-left: 20px;
  }
</style>